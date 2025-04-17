using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models.Images;
using Unite.Images.Feed.Web.Models.Images.Binders;
using Unite.Images.Feed.Web.Submissions;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/entries/mr")]
[Authorize(Policy = Policies.Data.Writer)]
public class MrImagesController : Controller
{
    private readonly ImagesSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public MrImagesController(
        ImagesSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindMrImagesSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] MrImageModel[] models, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddMrImagesSubmission(models);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.MR, submissionId, taskStatus);

        return Ok(taskId);
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(MrImageTsvModelsBinder))] MrImageModel[] models, [FromQuery] bool review = true)
    {
        return Post(models, review);
    }
}
