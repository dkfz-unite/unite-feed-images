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


    [HttpPost]
    public IActionResult Post([FromBody]MriImageModel[] models)
    {
        var submissionId = _submissionService.AddMriImagesSubmission(models);

        _submissionTaskService.CreateTask(SubmissionTaskType.MRI, submissionId);

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(MriImageTsvModelsBinder))]MriImageModel[] models)
    {
        return Post(models);
    }
}
