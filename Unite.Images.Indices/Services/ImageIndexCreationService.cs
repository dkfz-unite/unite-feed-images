using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Images.Indices.Services.Mappers;
using Unite.Indices.Entities.Images;
using Unite.Indices.Services;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Images.Indices.Services;

public class ImageIndexCreationService : IIndexCreationService<ImageIndex>
{
    private readonly DomainDbContext _dbContext;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;


    public ImageIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
    }


    public ImageIndex CreateIndex(object key)
    {
        var imageId = (int)key;

        return CreateImageIndex(imageId);
    }


    private ImageIndex CreateImageIndex(int imageId)
    {
        var image = LoadImage(imageId);

        if (image == null)
        {
            return null;
        }

        var index = CreateImageIndex(image);

        return index;
    }

    private ImageIndex CreateImageIndex(Image image)
    {
        var diagnosisDate = image.Donor.ClinicalData?.DiagnosisDate;

        var index = new ImageIndex();

        _imageIndexMapper.Map(image, index, diagnosisDate);

        index.Donor = CreateDonorIndex(image.DonorId);
        index.Specimens = CreateSpecimenIndices(image.DonorId, diagnosisDate);
        index.Data = CreateDataIndex(image.DonorId);

        var stats = LoadGenomicStats(image.DonorId);

        index.NumberOfGenes = stats.NumberOfGenes;
        index.NumberOfSsms = stats.NumberOfSsms;
        index.NumberOfCnvs = stats.NumberOfCnvs;
        index.NumberOfSvs = stats.NumberOfSvs;

        return index;
    }

    private Image LoadImage(int imageId)
    {
        var image = _dbContext.Set<Image>()
            .Include(image => image.MriImage)
            .Include(image => image.Donor)
                .ThenInclude(donor => donor.ClinicalData)
            .FirstOrDefault(image => image.Id == imageId);

        return image;
    }


    private DonorIndex CreateDonorIndex(int donorId)
    {
        var donor = LoadDonor(donorId);

        if (donor == null)
        {
            return null;
        }

        var index = CreateDonorIndex(donor);

        return index;
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        var index = new DonorIndex();

        _donorIndexMapper.Map(donor, index);

        return index;
    }

    private Donor LoadDonor(int donorId)
    {
        var donor = _dbContext.Set<Donor>()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeStudies()
            .IncludeProjects()
            .FirstOrDefault(donor => donor.Id == donorId);

        return donor;
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? diagnosisDate)
    {
        var specimens = LoadSpecimens(donorId);

        if (specimens == null)
        {
            return null;
        }

        var indices = specimens
            .Select(specimen => CreateSpecimenIndex(specimen, diagnosisDate))
            .ToArray();

        return indices;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index = new SpecimenIndex();

        _specimenIndexMapper.Map(specimen, index, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        // Images can be associated only with donor derived tumor tissues
        var specimens = _dbContext.Set<Specimen>()
            .IncludeTissue()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specime => specime.ParentId == null)
            .Where(specimen => specimen.Tissue != null && specimen.Tissue.TypeId == TissueType.Tumor)
            .ToArray();

        return specimens;
    }


    private DataIndex CreateDataIndex(int donorId)
    {
        var specimenIds = LoadSpecimenIds(donorId);

        var index = new DataIndex();

        index.Clinical = _dbContext.Set<ClinicalData>()
            .Where(clinicalData => clinicalData.DonorId == donorId)
            .Any();

        index.Treatments = _dbContext.Set<Treatment>()
            .Where(treatment => treatment.DonorId == donorId)
            .Any();

        index.Tissues = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.Tissue)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.Tissue != null && specimen.Tissue.TypeId != TissueType.Control)
            .Any();

        index.TissuesMolecular = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.MolecularData)
            .Include(specimen => specimen.Tissue)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.MolecularData != null)
            .Where(specimen => specimen.Tissue != null && specimen.Tissue.TypeId != TissueType.Control)
            .Any();

        index.Ssms = CheckVariants<SSM.Variant, SSM.VariantOccurrence>(specimenIds);

        index.Cnvs = CheckVariants<CNV.Variant, CNV.VariantOccurrence>(specimenIds);

        index.Svs = CheckVariants<SV.Variant, SV.VariantOccurrence>(specimenIds);

        index.GeneExp = CheckGeneExp(specimenIds);

        index.GeneExpSc = false;

        return index;
    }


    private record GenomicStats(int NumberOfGenes, int NumberOfSsms, int NumberOfCnvs, int NumberOfSvs);

    private GenomicStats LoadGenomicStats(int donorId)
    {
        var specimenIds = LoadSpecimenIds(donorId);
        var ssmIds = LoadVariantIds<SSM.Variant, SSM.VariantOccurrence>(specimenIds);
        var cnvIds = LoadVariantIds<CNV.Variant, CNV.VariantOccurrence>(specimenIds);
        var svIds = LoadVariantIds<SV.Variant, SV.VariantOccurrence>(specimenIds);
        var ssmGeneIds = LoadGeneIds<SSM.Variant, SSM.AffectedTranscript>(ssmIds);
        var cnvGeneIds = LoadGeneIds<CNV.Variant, CNV.AffectedTranscript>(cnvIds, affectedTranscript => affectedTranscript.Variant.TypeId != CNV.Enums.CnvType.Neutral);
        var svGeneIds = LoadGeneIds<SV.Variant, SV.AffectedTranscript>(svIds);
        var geneIds = Array.Empty<int>().Union(ssmGeneIds).Union(cnvGeneIds).Union(svGeneIds).ToArray();

        return new GenomicStats(geneIds.Length, ssmIds.Length, cnvIds.Length, svIds.Length);
    }

    /// <summary>
    /// Loads identifiers of donor derived specimens associated with given donor.
    /// </summary>
    /// <param name="donorId">Donor identifier.</param>
    /// <returns>Array of specimens identifiers.</returns>
    private int[] LoadSpecimenIds(int donorId)
    {
        // Images can be associated only with donor derived tumor tissues
        var ids = _dbContext.Set<Specimen>()
            .IncludeTissue()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.ParentId == null)
            .Where(specimen => specimen.Tissue != null && specimen.Tissue.TypeId == TissueType.Tumor)
            .Select(specimen => specimen.Id)
            .Distinct()
            .ToArray();

        return ids;
    }

    /// <summary>
    /// Loads identifiers of genes affected by given variants.
    /// </summary>
    /// <param name="variantIds">Varians identifiers.</param>
    /// <param name="filter">Affected transcript filter.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TAffectedTranscript">Variant affected transcript type.</typeparam>
    /// <returns>Array of genes identifiers.</returns>
    private int[] LoadGeneIds<TVariant, TAffectedTranscript>(long[] variantIds, Expression<Func<TAffectedTranscript, bool>> filter = null)
        where TVariant : Variant
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Data.Entities.Genome.Transcript>
    {
        Expression<Func<TAffectedTranscript, bool>> selectorPredicate = (affectedTranscript => variantIds.Contains(affectedTranscript.VariantId));
        Expression<Func<TAffectedTranscript, bool>> filterPredicate = filter ?? (affectedTranscript => true);

        var ids = _dbContext.Set<TAffectedTranscript>()
            .Where(selectorPredicate)
            .Where(filterPredicate)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        return ids;
    }

    /// <summary>
    /// Loads identifiers of variants of given type occurring in given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimens identifiers.</param>
    /// <param name="filter">Variant occurrence filter.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantOccurrence">Variant occurrence type.</typeparam>
    /// <returns>Array of variants identifiers.</returns>
    private long[] LoadVariantIds<TVariant, TVariantOccurrence>(int[] specimenIds, Expression<Func<TVariantOccurrence, bool>> filter = null)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
    {
        Expression<Func<TVariantOccurrence, bool>> selectorPredicate = (occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId));
        Expression<Func<TVariantOccurrence, bool>> filterPredicate = filter ?? (occurrence => true);

        var ids = _dbContext.Set<TVariantOccurrence>()
            .Where(selectorPredicate)
            .Where(filterPredicate)
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return ids;
    }

    /// <summary>
    /// Checks if variants data of given type is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantOccurrence">Variant occurrence type.</typeparam>
    /// <returns>'true' if variants data exists or 'false' otherwise.</returns>
    private bool CheckVariants<TVariant, TVariantOccurrence>(int[] specimenIds)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
    {
        return _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .Any();
    }

    /// <summary>
    /// Checks if gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExp(int[] specimenIds)
    {
        return _dbContext.Set<GeneExpression>()
            .Any(expression => specimenIds.Contains(expression.AnalysedSample.Sample.SpecimenId));
    }
}
