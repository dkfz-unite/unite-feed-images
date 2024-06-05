using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Repositories.Constants;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Images;
using Unite.Mapping;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Images.Indices.Services;

public class ImageIndexCreationService
{
    private record GenomicStats(int NumberOfGenes, int NumberOfSsms, int NumberOfCnvs, int NumberOfSvs);

    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly DonorsRepository _donorsRepository;
    private readonly ImagesRepository _imagesRepository;
    private readonly SpecimensRepository _specimensRepository;


    public ImageIndexCreationService(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _donorsRepository = new DonorsRepository(dbContextFactory);
        _imagesRepository = new ImagesRepository(dbContextFactory);
        _specimensRepository = new SpecimensRepository(dbContextFactory);
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
            return null;

        return CreateImageIndex(image);
    }

    private ImageIndex CreateImageIndex(Image image)
    {
        var diagnosisDate = image.Donor.ClinicalData?.DiagnosisDate;
        var stats = LoadGenomicStats(image.DonorId);

        var index = new ImageIndex();

        ImageIndexMapper.Map(image, index, diagnosisDate);

        index.DonorId = image.DonorId;

        index.Donor = CreateDonorIndex(image.DonorId);
        index.Specimens = CreateSpecimenIndices(image.DonorId, diagnosisDate);
        index.Data = CreateDataIndex(image.TypeId, image.DonorId);

        index.NumberOfGenes = stats.NumberOfGenes;
        index.NumberOfSsms = stats.NumberOfSsms;
        index.NumberOfCnvs = stats.NumberOfCnvs;
        index.NumberOfSvs = stats.NumberOfSvs;

        return index;
    }

    private Image LoadImage(int imageId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            .Include(image => image.Donor)
                .ThenInclude(donor => donor.ClinicalData)
            .FirstOrDefault(image => image.Id == imageId);
    }


    private DonorIndex CreateDonorIndex(int donorId)
    {
        var donor = LoadDonor(donorId);

        if (donor == null)
            return null;

        return CreateDonorIndex(donor);
    }

    private static DonorIndex CreateDonorIndex(Donor donor)
    {
        return DonorIndexMapper.CreateFrom<DonorIndex>(donor);
    }

    private Donor LoadDonor(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeStudies()
            .IncludeProjects()
            .FirstOrDefault(donor => donor.Id == donorId);
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? diagnosisDate)
    {
        var specimens = LoadSpecimens(donorId);

        return specimens.Select(specimen => CreateSpecimenIndex(specimen, diagnosisDate)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, diagnosisDate);

        index.Samples = CreateSampleIndices(specimen.Id, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMaterial()
            .IncludeMolecularData()
            .Where(Predicates.IsImageRelatedSpecimen)
            .Where(specimen => specimen.DonorId == donorId)
            .ToArray();
    }


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var samples = LoadSamples(specimenId);

        return samples.Select(sample => CreateSampleIndex(sample, diagnosisDate)).ToArrayOrNull();
    }

    private static SampleIndex CreateSampleIndex(Sample sample, DateOnly? diagnosisDate)
    {
        var index = SampleIndexMapper.CreateFrom<SampleIndex>(sample, diagnosisDate);

        return sample.Resources?.Select(resource => ResourceIndexMapper.CreateFrom<ResourceIndex>(resource)).ToArrayOrNull();
    }

    private Sample[] LoadSamples(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => sample.SpecimenId == specimenId)
            .ToArray();
    }


    private DataIndex CreateDataIndex(ImageType type, int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorId),
            Treatments = CheckTreatments(donorId),
            Mris = type == ImageType.MRI,
            Cts = type == ImageType.CT,
            Materials = CheckSpecimen(donorId),
            MaterialsMolecular = CheckMolecularData(donorId),
            Ssms = CheckVariants<SSM.Variant, SSM.VariantEntry>(specimenIds),
            Cnvs = CheckVariants<CNV.Variant, CNV.VariantEntry>(specimenIds),
            Svs = CheckVariants<SV.Variant, SV.VariantEntry>(specimenIds),
            GeneExp = CheckGeneExp(specimenIds),
            GeneExpSc = CheckGeneExpSc(specimenIds)
        };
    }

    private GenomicStats LoadGenomicStats(int donorId)
    {
        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;
        var ssmIds = _specimensRepository.GetRelatedVariants<SSM.Variant>(specimenIds).Result;
        var cnvIds = _specimensRepository.GetRelatedVariants<CNV.Variant>(specimenIds).Result;
        var svIds = _specimensRepository.GetRelatedVariants<SV.Variant>(specimenIds).Result;
        var geneIds = _specimensRepository.GetVariantRelatedGenes(specimenIds).Result;

        return new GenomicStats(geneIds.Length, ssmIds.Length, cnvIds.Length, svIds.Length);
    }


    private bool CheckClinicalData(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<ClinicalData>()
            .AsNoTracking()
            .Where(clinical => clinical.DonorId == donorId)
            .Any();
    }

    private bool CheckTreatments(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Treatment>()
            .AsNoTracking()
            .Where(treatment => treatment.DonorId == donorId)
            .Any();
    }

    private bool CheckSpecimen(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(Predicates.IsImageRelatedSpecimen)
            .Where(specimen => specimen.DonorId == donorId)
            .Any();
    }

    private bool CheckMolecularData(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(Predicates.IsImageRelatedSpecimen)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.MolecularData != null)
            .Any();
    }

    /// <summary>
    /// Checks if variants data of given type is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantEntry">Variant occurrence type.</typeparam>
    /// <returns>'true' if variants data exists or 'false' otherwise.</returns>
    private bool CheckVariants<TVariant, TVariantEntry>(int[] specimenIds)
        where TVariant : Variant
        where TVariantEntry : VariantEntry<TVariant>
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TVariantEntry>()
            .AsNoTracking()
            .Where(entry => specimenIds.Contains(entry.Sample.SpecimenId))
            .Select(entry => entry.EntityId)
            .Distinct()
            .Any();
    }

    /// <summary>
    /// Checks if bulk gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if bulk gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExp(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Any(expression => specimenIds.Contains(expression.Sample.SpecimenId));
    }

    /// <summary>
    /// Checks if single cell gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers
    /// <returns>'true' if single cell gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExpSc(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Any(sample => specimenIds.Contains(sample.SpecimenId) && sample.Analysis.TypeId == AnalysisType.RNASeqSc);
    }
}
