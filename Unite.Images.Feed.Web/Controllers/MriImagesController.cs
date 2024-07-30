using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models;
using Unite.Images.Feed.Web.Models.Binders;
using Unite.Images.Feed.Web.Models.Converters;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/entries/mri")]
[Authorize(Policy = Policies.Data.Writer)]
public class MriImagesController : Controller
{
    private readonly ImagesWriter _dataWriter;
    private readonly ImageIndexingTasksService _tasksService;
    private readonly ILogger _logger;

    private readonly MriImageModelConverter _converter = new();

    public MriImagesController(
        ImagesWriter dataWriter,
        ImageIndexingTasksService tasksService,
        ILogger<MriImagesController> logger)
    {
        _dataWriter = dataWriter;
        _tasksService = tasksService;
        _logger = logger;
    }


    [HttpPost]
    public IActionResult Post([FromBody]MriImageModel[] models)
    {
        var data = models.Select(model => _converter.Convert(model)).ToArray();

        _dataWriter.SaveData(data, out var audit);
        _tasksService.PopulateTasks(audit.Images);
        _logger.LogInformation("{audit}", audit.ToString());

        return Ok();
    }

    [HttpPost("tsv")]
    public IActionResult PostTsv([ModelBinder(typeof(MriImageTsvModelsBinder))]MriImageModel[] models)
    {
        return Post(models);
    }
}
