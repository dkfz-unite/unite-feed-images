using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Images.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Images;

namespace Unite.Images.Feed.Web.Handlers;

public class ImagesIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ImageIndexCreator _indexCreator;
    private readonly IIndexService<ImageIndex> _indexingService;
    private readonly ILogger _logger;


    public ImagesIndexingHandler(
        TasksProcessingService taskProcessingService,
        ImageIndexCreator indexCreator,
        IIndexService<ImageIndex> indexingService,
        ILogger<ImagesIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexCreator = indexCreator;
        _indexingService = indexingService;
        _logger = logger;
    }


    public async Task Prepare()
    {
        await _indexingService.UpdateIndex();
    }

    public async Task Handle(int bucketSize)
    {
        await ProcessImageIndexingTasks(bucketSize);
    }


    private async Task ProcessImageIndexingTasks(int bucketSize)
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.Image, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<ImageIndex>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreator.CreateIndex(id);

                if (index == null)
                    indicesToDelete.Add($"{id}");
                else
                    indicesToCreate.Add(index);
            });

            if (indicesToDelete.Any())
                await _indexingService.DeleteRange(indicesToDelete);

            if (indicesToCreate.Any())
                await _indexingService.AddRange(indicesToCreate);

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} images in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
