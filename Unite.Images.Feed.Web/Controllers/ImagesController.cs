using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models.Images;
using Unite.Images.Feed.Web.Models.Images.Binders;
using Unite.Images.Feed.Web.Models.Images.Converters;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class ImagesController : Controller
{
    private readonly ImageDataWriter _dataWriter;
    private readonly ImageIndexingTasksService _indexingTasksService;
    private readonly ILogger _logger;

    private readonly ImageModelConverter _converter;


    public ImagesController(
        ImageDataWriter dataWriter,
        ImageIndexingTasksService indexingTasksService,
        ILogger<ImagesController> logger)
    {
        _dataWriter = dataWriter;
        _indexingTasksService = indexingTasksService;
        _logger = logger;

        _converter = new ImageModelConverter();
    }


    [HttpPost]
    [Consumes("application/json", new[] { "application/jsonc" })]
    public IActionResult Post([FromBody] ImageModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model));

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTasksService.PopulateTasks(audit.Images);

        return Ok();
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(MriImageTsvModelBinder))] ImageModel[] models)
    {
        var dataModels = models.Select(model => _converter.Convert(model));

        _dataWriter.SaveData(dataModels, out var audit);

        _logger.LogInformation(audit.ToString());

        _indexingTasksService.PopulateTasks(audit.Images);

        return Ok();
    }
}
