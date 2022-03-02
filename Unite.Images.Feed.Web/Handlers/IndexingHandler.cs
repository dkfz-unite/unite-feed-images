using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Feed.Web.Services;
using Unite.Indices.Entities.Images;
using Unite.Indices.Services;

namespace Unite.Images.Feed.Web.Handlers
{
    public class IndexingHandler
    {
        private readonly TasksProcessingService _taskProcessingService;
        private readonly IIndexCreationService<ImageIndex> _indexCreationService;
        private readonly IIndexingService<ImageIndex> _indexingService;
        private readonly ILogger _logger;


        public IndexingHandler(
            TasksProcessingService taskProcessingService,
            IIndexCreationService<ImageIndex> indexCreationService,
            IIndexingService<ImageIndex> indexingService,
            ILogger<IndexingHandler> logger)
        {
            _taskProcessingService = taskProcessingService;
            _indexCreationService = indexCreationService;
            _indexingService = indexingService;
            _logger = logger;
        }


        public void Handle(int bucketSize)
        {
            ProcessImageIndexingTasks(bucketSize);
        }


        private void ProcessImageIndexingTasks(int bucketSize)
        {
            _taskProcessingService.Process(TaskType.Indexing, TaskTargetType.Image, bucketSize, (tasks) =>
            {
                _logger.LogInformation($"Indexing {tasks.Length} images");

                var indices = tasks.Select(task =>
                {
                    var id = int.Parse(task.Target);

                    var index = _indexCreationService.CreateIndex(id);

                    return index;

                }).ToArray();

                _indexingService.IndexMany(indices);

                _logger.LogInformation($"Indexing of {tasks.Length} images completed");
            });
        }
    }
}
