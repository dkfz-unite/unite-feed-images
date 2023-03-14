using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
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

namespace Unite.Images.Indices.Services
{
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
            
            var stats = LoadGenomicStats(image.DonorId);

            var index = new ImageIndex();

            _imageIndexMapper.Map(image, index, diagnosisDate);

            index.Donor = CreateDonorIndex(image.DonorId);
            index.Specimens = CreateSpecimenIndices(image.DonorId, diagnosisDate);
            index.NumberOfGenes = stats.NumberOfGenes;
            index.NumberOfMutations = stats.NumberOfMutations;
            index.NumberOfCopyNumberVariants = stats.NumberOfCopyNumberVariants;
            index.NumberOfStructuralVariants = stats.NumberOfStructuralVariants;

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


        private record GenomicStats(int NumberOfGenes, int NumberOfMutations, int NumberOfCopyNumberVariants, int NumberOfStructuralVariants);

        private GenomicStats LoadGenomicStats(int donorId)
        {
            var specimenIds = LoadSpecimenIds(donorId);
            var ssmIds = LoadVariantIds<SSM.Variant, SSM.VariantOccurrence>(specimenIds);
            var cnvIds = LoadVariantIds<CNV.Variant, CNV.VariantOccurrence>(specimenIds);
            var svIds = LoadVariantIds<SV.Variant, SV.VariantOccurrence>(specimenIds);
            var ssmGeneIds = LoadGeneIds<SSM.Variant, SSM.AffectedTranscript>(ssmIds);
            var cnvGeneIds = LoadGeneIds<CNV.Variant, CNV.AffectedTranscript>(ssmIds);
            var svGeneIds = LoadGeneIds<SV.Variant, SV.AffectedTranscript>(ssmIds);
            var geneIds = ssmGeneIds.Union(cnvGeneIds).Union(svGeneIds).ToArray();

            return new GenomicStats(geneIds.Length, ssmIds.Length, cnvIds.Length, svIds.Length);
        }

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

        private int[] LoadGeneIds<TVariant, TAffectedTranscript>(long[] variantIds)
            where TVariant : Variant
            where TAffectedTranscript : VariantAffectedFeature<TVariant, Data.Entities.Genome.Transcript>
        {
            var ids = _dbContext.Set<TAffectedTranscript>()
                .Where(affectedTranscript => variantIds.Contains(affectedTranscript.VariantId))
                .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
                .Distinct()
                .ToArray();

            return ids;
        }

        private long[] LoadVariantIds<TVariant, TVariantOccurrence>(int[] specimenIds)
            where TVariant : Variant
            where TVariantOccurrence : VariantOccurrence<TVariant>
        {
            var ids = _dbContext.Set<TVariantOccurrence>()
                .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
                .Select(occurrence => occurrence.VariantId)
                .Distinct()
                .ToArray();

            return ids;
        }
    }
}
