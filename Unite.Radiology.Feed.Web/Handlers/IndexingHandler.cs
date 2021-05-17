using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Radiology.Feed.Web.Services;

namespace Unite.Radiology.Feed.Web.Handlers
{
    public class IndexingHandler
    {
        private readonly TaskProcessingService _taskProcessingService;
        private readonly ILogger _logger;


        public IndexingHandler(
            TaskProcessingService taskProcessingService,
            ILogger<IndexingHandler> logger)
        {
            _taskProcessingService = taskProcessingService;
            _logger = logger;
        }


        public void Handle(int bucketSize)
        {
            ProcessDonorIndexingTasks(bucketSize);
        }


        private void ProcessDonorIndexingTasks(int bucketSize)
        {
            //_taskProcessingService.Process(TaskType.Indexing, TaskTargetType.???, bucketSize, (tasks) =>
            //{
            //    _logger.LogInformation($"Indexing {tasks.Length} donors");

            //    _logger.LogInformation($"Indexing of {tasks.Length} donors completed");
            //});
        }
    }
}

