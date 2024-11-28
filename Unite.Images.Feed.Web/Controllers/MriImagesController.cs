using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models;
using Unite.Images.Feed.Web.Models.Binders;
using Unite.Images.Feed.Web.Submissions;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/entries/mri")]
[Authorize(Policy = Policies.Data.Writer)]
public class MriImagesController : Controller
{
    private readonly ImagesSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public MriImagesController(
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

        var submission = _submissionService.FindMriImagesSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] MriImageModel[] models, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddMriImagesSubmission(models);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.MRI, submissionId, taskStatus);

        return Ok(taskId);
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(MriImageTsvModelsBinder))] MriImageModel[] models, [FromQuery] bool review = true)
    {
        return Post(models, review);
    }
}
