using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Models.Images;
using Unite.Images.Feed.Web.Models.Images.Converters;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Feed.Web.Submissions;

namespace Unite.Images.Feed.Web.Handlers.Submission;

public class MrImagesSubmissionHandler
{
    private readonly ImagesWriter _dataWriter;
    private readonly ImageIndexingTasksService _tasksService;
    private readonly ImagesSubmissionService _submissionService;
    private readonly TasksProcessingService _tasksProcessingService;

    private readonly MrImageModelConverter _modelConverter;

    private readonly ILogger _logger;

    public MrImagesSubmissionHandler(
           ImagesWriter dataWriter,
           ImageIndexingTasksService tasksService,
           ImagesSubmissionService submissionService,
           TasksProcessingService tasksProcessingService,
           ILogger<MrImagesSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _submissionService = submissionService;
        _tasksProcessingService = tasksProcessingService;
        _logger = logger;

        _modelConverter = new MrImageModelConverter();
    }

     public void Handle()
    {
        ProcessSubmissionTasks();
    }

     private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _tasksProcessingService.Process(SubmissionTaskType.MR, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed MR images data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindMrImagesSubmission(submissionId);
        var convertedData = submittedData.Select<MrImageModel, Data.Models.ImageModel>((mriModel) => _modelConverter.Convert(mriModel));

        _dataWriter.SaveData(convertedData, out var audit);
        _tasksService.PopulateTasks(audit.Images);
        _submissionService.DeleteMrImagesSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}