using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Images;

namespace Unite.Images.Feed.Web.Handlers;

public class ImagesIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ImageIndexCreationService _indexCreationService;
    private readonly IIndexService<ImageIndex> _indexingService;
    private readonly ILogger _logger;


    public ImagesIndexingHandler(
        TasksProcessingService taskProcessingService,
        ImageIndexCreationService indexCreationService,
        IIndexService<ImageIndex> indexingService,
        ILogger<ImagesIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexCreationService = indexCreationService;
        _indexingService = indexingService;
        _logger = logger;
    }


    public void Prepare()
    {
        _indexingService.UpdateIndex().GetAwaiter().GetResult();
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

            _logger.LogInformation("Indexing {number} images", tasks.Length);

            stopwatch.Restart();

            var grouped = tasks.DistinctBy(task => task.Target);

            var indices = grouped.Select(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                return index;

            }).ToArray();

            _indexingService.AddRange(indices);

            stopwatch.Stop();

            _logger.LogInformation("Indexing of {number} images completed in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
