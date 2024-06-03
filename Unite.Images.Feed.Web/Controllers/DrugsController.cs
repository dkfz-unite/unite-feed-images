using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Radiomics;
using Unite.Images.Feed.Web.Models.Radiomics.Binders;
using Unite.Images.Feed.Web.Models.Radiomics.Converters;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/analysis/drugs")]
[Authorize(Policy = Policies.Data.Writer)]
public class DrugsController : Controller
{
    private readonly AnalysisWriter _dataWriter;
    private readonly ImageIndexingTasksService _taskService;
    private readonly ILogger<DrugsController> _logger;

    private readonly AnalysisModelConverter _converter = new();


    public DrugsController(
        AnalysisWriter dataWriter, 
        ImageIndexingTasksService taskService, 
        ILogger<DrugsController> logger)
    {
        _dataWriter = dataWriter;
        _taskService = taskService;
        _logger = logger;
    }


    [HttpPost("")]
    public IActionResult Post([FromBody]AnalysisModel<FeatureModel> model)
    {
        return PostData(model);
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelBinder))]AnalysisModel<FeatureModel> model)
    {
        return PostData(model);
    }


    private IActionResult PostData(AnalysisModel<FeatureModel> model)
    {
        var data = _converter.Convert(model);

        _dataWriter.SaveData(data, out var audit);
        _taskService.PopulateTasks(audit.Images);
        _logger.LogInformation("{audit}", audit.ToString());

        return Ok();
    }
}
