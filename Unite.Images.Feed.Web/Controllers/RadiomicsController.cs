using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Radiomics;
using Unite.Images.Feed.Web.Models.Radiomics.Binders;
using Unite.Images.Feed.Web.Submissions;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/analysis/radiomics")]
[Authorize(Policy = Policies.Data.Writer)]
public class RadiomicsController : Controller
{
    private readonly ImagesSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public RadiomicsController(
        ImagesSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody]AnalysisModel<FeatureModel> model)
    {
        var submissionId = _submissionService.AddRadiomicsSubmission(model);

        _submissionTaskService.CreateTask(SubmissionTaskType.IMG_RAD, submissionId);

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelBinder))]AnalysisModel<FeatureModel> model)
    {
        return Post(model);
    }
}
