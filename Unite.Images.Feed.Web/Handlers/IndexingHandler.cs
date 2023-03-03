using System.Diagnostics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Indices.Entities.Images;
using Unite.Indices.Services;

namespace Unite.Images.Feed.Web.Handlers;

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


    public void Prepare()
    {
        _indexingService.UpdateMapping().GetAwaiter().GetResult();
    }

    public void Handle(int bucketSize)
    {
        ProcessImageIndexingTasks(bucketSize);
    }


    private void ProcessImageIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(IndexingTaskType.Image, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasSubmissionTasks() || _taskProcessingService.HasAnnotationTasks())
            {
                return false;
            }

            _logger.LogInformation($"Indexing {tasks.Length} images");

            stopwatch.Restart();

            var grouped = tasks.DistinctBy(task => task.Target);

            var indices = grouped.Select(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                return index;

            }).ToArray();

            _indexingService.IndexMany(indices);

            stopwatch.Stop();

            _logger.LogInformation($"Indexing of {tasks.Length} images completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }
}
