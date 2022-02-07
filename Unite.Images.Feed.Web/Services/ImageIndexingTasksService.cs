using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Images.Feed.Web.Services
{
    public class ImageIndexingTasksService
    {
        private const int BUCKET_SIZE = 1000;

        private readonly DomainDbContext _dbContext;


        public ImageIndexingTasksService(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates only iamge indexing tasks for all existing images
        /// </summary>
        public void CreateTasks()
        {
            IterateEntities<Image, int>(image => true, image => image.Id, images =>
            {
                CreateImageIndexingTasks(images);
            });
        }

        /// <summary>
        /// Creates only image indexing tasks for all images with given identifiers
        /// </summary>
        /// <param name="imageIds">Identifiers of images</param>
        public void CreateTasks(IEnumerable<int> imageIds)
        {
            IterateEntities<Image, int>(image => imageIds.Contains(image.Id), image => image.Id, images =>
            {
                CreateImageIndexingTasks(images);
            });
        }

        /// <summary>
        /// Populates all types of indexing tasks for images with given identifiers
        /// </summary>
        /// <param name="imageIds">Identifiers of images</param>
        public void PopulateTasks(IEnumerable<int> imageIds)
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


        private void CreateDonorIndexingTasks(IEnumerable<int> imageIds)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => imageIds.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            CreateTasks(TaskType.Indexing, TaskTargetType.Donor, donorIds);
        }

        private void CreateImageIndexingTasks(IEnumerable<int> imageIds)
        {
            CreateTasks(TaskType.Indexing, TaskTargetType.Image, imageIds);
        }

        private void CreateSpecimenIndexingTasks(IEnumerable<int> imageIds)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => imageIds.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Distinct()
                .ToArray();

            CreateTasks(TaskType.Indexing, TaskTargetType.Specimen, specimenIds);
        }

        private void CreateMutationIndexingTasks(IEnumerable<int> imageIds)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => imageIds.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(occurrence => specimenIds.Contains(occurrence.Sample.SpecimenId))
                .Select(occurrence => occurrence.MutationId)
                .Distinct()
                .ToArray();

            CreateTasks(TaskType.Indexing, TaskTargetType.Mutation, mutationIds);
        }

        private void CreateGeneIndexingTasks(IEnumerable<int> imageIds)
        {
            var donorIds = _dbContext.Set<Image>()
                .Where(image => imageIds.Contains(image.Id))
                .Select(image => image.DonorId)
                .Distinct()
                .ToArray();

            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(occurrence => specimenIds.Contains(occurrence.Sample.SpecimenId))
                .Select(occurrence => occurrence.MutationId)
                .Distinct()
                .ToArray();

            var geneIds = _dbContext.Set<AffectedTranscript>()
                .Where(affectedTranscript => mutationIds.Contains(affectedTranscript.MutationId))
                .Select(affectedTranscripts => affectedTranscripts.Transcript.GeneId)
                .Distinct()
                .ToArray();

            CreateTasks(TaskType.Indexing, TaskTargetType.Gene, geneIds);
        }


        protected void CreateTasks<T>(
            TaskType type,
            TaskTargetType targetType,
            IEnumerable<T> keys)
        {
            var tasks = keys
                .Select(key => new Task
                {
                    TypeId = type,
                    TargetTypeId = targetType,
                    Target = key.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.AddRange(tasks);
            _dbContext.SaveChanges();
        }


        protected void IterateEntities<T, TKey>(
            Expression<Func<T, bool>> condition,
            Expression<Func<T, TKey>> selector,
            Action<IEnumerable<TKey>> handler)
            where T : class
        {
            var position = 0;

            var entities = Enumerable.Empty<TKey>();

            do
            {
                entities = _dbContext.Set<T>()
                    .Where(condition)
                    .Skip(position)
                    .Take(BUCKET_SIZE)
                    .Select(selector)
                    .ToArray();

                handler.Invoke(entities);

                position += entities.Count();

            }
            while (entities.Count() == BUCKET_SIZE);
        }
    }
}
