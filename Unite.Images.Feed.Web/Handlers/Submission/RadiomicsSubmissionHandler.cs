using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Feed.Web.Submissions;



namespace Unite.Images.Feed.Web.Handlers.Submission;

public class RadiomicsSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly ImageIndexingTasksService _tasksService;
    private readonly ImagesSubmissionService _submissionService;
    private readonly TasksProcessingService _tasksProcessingService;

    private readonly Models.Radiomics.Converters.AnalysisModelConverter _modelConverter;

    private readonly ILogger _logger;

    public RadiomicsSubmissionHandler(
           AnalysisWriter dataWriter,
           ImageIndexingTasksService tasksService,
           ImagesSubmissionService submissionService,
           TasksProcessingService tasksProcessingService,
           ILogger<RadiomicsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _submissionService = submissionService;
        _tasksProcessingService = tasksProcessingService;
        _logger = logger;

        _modelConverter = new Models.Radiomics.Converters.AnalysisModelConverter();
    }

    public void Handle()
    {
        ProcessSubmissionTasks();
    }

     private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _tasksProcessingService.Process(SubmissionTaskType.IMG_RAD, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed Radiomics data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

     private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindRadiomicsSubmission(submissionId);
        var convertedData = _modelConverter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _tasksService.PopulateTasks(audit.Images);
        _submissionService.DeleteRadiomicsSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}