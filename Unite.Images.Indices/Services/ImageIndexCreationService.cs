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
        private readonly VariantIndexMapper _variantIndexMapper;


        public ImageIndexCreationService(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _donorIndexMapper = new DonorIndexMapper();
            _imageIndexMapper = new ImageIndexMapper();
            _specimenIndexMapper = new SpecimenIndexMapper();
            _variantIndexMapper = new VariantIndexMapper();
        }


        public ImageIndex CreateIndex(object key)
        {
            var imageId = (int)key;

            return CreateImageIndex(imageId);
        }


        private ImageIndex CreateImageIndex(int imageId)
        {
            var image = LoadImage(imageId);

            var index = CreateImageIndex(image);

            return index;
        }

        private ImageIndex CreateImageIndex(Image image)
        {
            var index = new ImageIndex();

            var diagnosisDate = image.Donor.ClinicalData?.DiagnosisDate;

            _imageIndexMapper.Map(image, index, diagnosisDate);

            index.Donor = CreateDonorIndex(image.DonorId);

            index.Specimens = CreateSpecimenIndices(image.DonorId, diagnosisDate);

            index.NumberOfGenes += index.Specimens
                .SelectMany(specimen => specimen.Variants.Where(variant => variant.AffectedTranscripts != null))
                .SelectMany(variant => variant.AffectedTranscripts)
                .Select(affectedTranscript => affectedTranscript.Gene.Id)
                .Distinct()
                .Count();

            index.NumberOfMutations = index.Specimens
                .SelectMany(specimen => specimen.Variants)
                .Where(variant => variant.Mutation != null)
                .DistinctBy(variant => variant.Id)
                .Count();

            index.NumberOfCopyNumberVariants = index.Specimens
                .SelectMany(specimen => specimen.Variants)
                .Where(variant => variant.CopyNumberVariant != null)
                .DistinctBy(variant => variant.Id)
                .Count();

            index.NumberOfStructuralVariants = index.Specimens
                .SelectMany(specimen => specimen.Variants)
                .Where(variant => variant.StructuralVariant != null)
                .DistinctBy(variant => variant.Id)
                .Count();

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

            index.Variants = CreateVariantIndices(specimen.Id);

            return index;
        }

        private Specimen[] LoadSpecimens(int donorId)
        {
            // Images can be associated only with tumor tissues
            var specimens = _dbContext.Set<Specimen>()
                .IncludeTissue()
                .IncludeMolecularData()
                .IncludeDrugScreeningData()
                .Where(specimen =>
                    specimen.Tissue != null &&
                    specimen.Tissue.TypeId == TissueType.Tumor &&
                    specimen.DonorId == donorId)
                .ToArray();

            return specimens;
        }


        private VariantIndex[] CreateVariantIndices(int specimenId)
        {
            var mutations = LoadVariants<SSM.Variant, SSM.VariantOccurrence>(specimenId);
            var copyNumberVariants = LoadVariants<CNV.Variant, CNV.VariantOccurrence>(specimenId);
            var structuralVariants = LoadVariants<SV.Variant, SV.VariantOccurrence>(specimenId);

            var indices = new List<VariantIndex>();

            if (mutations != null)
            {
                indices.AddRange(mutations.Select(variant => CreateVariantIndex(variant)));
            }

            if (copyNumberVariants != null)
            {
                indices.AddRange(copyNumberVariants.Select(variant => CreateVariantIndex(variant)));
            }

            if (structuralVariants != null)
            {
                indices.AddRange(structuralVariants.Select(variant => CreateVariantIndex(variant)));
            }

            return indices.Any() ? indices.ToArray() : null;
        }

        private VariantIndex CreateVariantIndex<TVariant>(TVariant variant)
            where TVariant : Variant
        {
            var index = new VariantIndex();

            _variantIndexMapper.Map(variant, index);

            return index;
        }

        private TVariant[] LoadVariants<TVariant, TVariantOccurrence>(int specimenId)
            where TVariant : Variant
            where TVariantOccurrence : VariantOccurrence<TVariant>
        {
            var variantIds = _dbContext.Set<TVariantOccurrence>()
                .Where(occurrence => occurrence.AnalysedSample.Sample.SpecimenId == specimenId)
                .Select(occurrence => occurrence.VariantId)
                .Distinct()
                .ToArray();

            var variants = _dbContext.Set<TVariant>()
                .IncludeAffectedTranscripts()
                .Where(variant => variantIds.Contains(variant.Id))
                .ToArray();

            return variants;
        }
    }
}
