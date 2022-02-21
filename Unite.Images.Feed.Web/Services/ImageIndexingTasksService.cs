using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;

namespace Unite.Images.Feed.Web.Services
{
    public class ImageIndexingTasksService : IndexingTaskService<Image, int>
    {
        protected override int BucketSize => 1000;

        public ImageIndexingTasksService(DomainDbContext dbContext) : base(dbContext)
        {
        }

        public override void CreateTasks()
        {
            IterateEntities<Image, int>(image => true, image => image.Id, images =>
            {
                CreateImageIndexingTasks(images);
            });
        }

        public override void CreateTasks(IEnumerable<int> imageIds)
        {
            IterateEntities<Image, int>(image => imageIds.Contains(image.Id), image => image.Id, images =>
            {
                CreateImageIndexingTasks(images);
            });
        }

        public override void PopulateTasks(IEnumerable<int> imageIds)
        {
            IterateEntities<Image, int>(image => imageIds.Contains(image.Id), image => image.Id, images =>
            {
                CreateDonorIndexingTasks(images);
                CreateImageIndexingTasks(images);
                CreateSpecimenIndexingTasks(images);
                CreateMutationIndexingTasks(images);
                CreateGeneIndexingTasks(images);
            });
        }


        protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => keys.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            return donorIds;
        }

        protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
        {
            return keys;
        }

        protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => keys.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            return specimenIds;
        }

        protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<int> keys)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => keys.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
                .Select(occurrence => occurrence.MutationId)
                .Distinct()
                .ToArray();

            return mutationIds;
        }

        protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => keys.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
                .Select(occurrence => occurrence.MutationId)
                .Distinct()
                .ToArray();

            var geneIds = _dbContext.Set<AffectedTranscript>()
                .Where(affectedTranscript => mutationIds.Contains(affectedTranscript.MutationId))
                .Where(affectedTranscript => affectedTranscript.Transcript.GeneId != null)
                .Select(affectedTranscripts => affectedTranscripts.Transcript.GeneId.Value)
                .Distinct()
                .ToArray();

            return geneIds;
        }
    }
}
