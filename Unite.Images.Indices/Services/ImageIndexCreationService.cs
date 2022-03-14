using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Images.Indices.Services.Mappers;
using Unite.Indices.Entities.Images;
using Unite.Indices.Services;

namespace Unite.Images.Indices.Services
{
    public class ImageIndexCreationService : IIndexCreationService<ImageIndex>
    {
        private readonly DomainDbContext _dbContext;
        private readonly DonorIndexMapper _donorIndexMapper;
        private readonly ImageIndexMapper _imageIndexMapper;
        private readonly SpecimenIndexMapper _specimenIndexMapper;
        private readonly MutationIndexMapper _mutationIndexMapper;


        public ImageIndexCreationService(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
            _donorIndexMapper = new DonorIndexMapper();
            _imageIndexMapper = new ImageIndexMapper();
            _specimenIndexMapper = new SpecimenIndexMapper();
            _mutationIndexMapper = new MutationIndexMapper();
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

            var diagnosisDate = image.Donor.ClinicalData.DiagnosisDate;

            _imageIndexMapper.Map(image, index, diagnosisDate);

            index.Donor = CreateDonorIndex(image.DonorId);

            index.Specimens = CreateSpecimenIndices(image.DonorId, diagnosisDate);

            index.NumberOfGenes = index.Specimens
                .SelectMany(specimen => specimen.Mutations.Where(mutation => mutation.AffectedTranscripts != null))
                .SelectMany(mutation => mutation.AffectedTranscripts)
                .Select(affectedTranscript => affectedTranscript.Transcript.Gene.Id)
                .Distinct()
                .Count();

            index.NumberOfMutations = index.Specimens
                .SelectMany(specimen => specimen.Mutations)
                .Select(mutation => mutation.Id)
                .Distinct()
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
                .IncludeWorkPackages()
                .FirstOrDefault(donor => donor.Id == donorId);

            return donor;
        }


        private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateTime? diagnosisDate)
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

        private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateTime? diagnosisDate)
        {
            var index = new SpecimenIndex();

            _specimenIndexMapper.Map(specimen, index, diagnosisDate);

            index.Mutations = CreateMutationIndices(specimen.Id);

            return index;
        }

        private Specimen[] LoadSpecimens(int donorId)
        {
            // Images can be associated only with tumor tissues
            var specimens = _dbContext.Set<Specimen>()
                .IncludeTissue()
                .IncludeMolecularData()
                .Where(specimen =>
                    specimen.Tissue != null &&
                    specimen.Tissue.TypeId == TissueType.Tumor &&
                    specimen.DonorId == donorId)
                .ToArray();

            return specimens;
        }


        private MutationIndex[] CreateMutationIndices(int specimenId)
        {
            var mutations = LoadMutations(specimenId);

            if (mutations == null)
            {
                return null;
            }

            var indices = mutations
                .Select(mutation => CreateMutationIndex(mutation))
                .ToArray();

            return indices;
        }

        private MutationIndex CreateMutationIndex(Mutation mutation)
        {
            var index = new MutationIndex();

            _mutationIndexMapper.Map(mutation, index);

            return index;
        }

        private Mutation[] LoadMutations(int specimenId)
        {
            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.SpecimenId == specimenId)
                .Select(mutationOccurrence => mutationOccurrence.MutationId)
                .Distinct()
                .ToArray();

            var mutations = _dbContext.Set<Mutation>()
                .IncludeAffectedTranscripts()
                .Where(mutation => mutationIds.Contains(mutation.Id))
                .ToArray();

            return mutations;
        }
    }
}
