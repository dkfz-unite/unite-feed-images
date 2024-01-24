using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Models.Converters;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

public class ImagesControllerBase : Controller
{
    protected readonly ImagesDataWriter _dataWriter;
    protected readonly ImageIndexingTasksService _indexingTasksService;
    protected readonly ILogger _logger;

    protected readonly ImageDataModelConverter _converter;

    public ImagesControllerBase(
        ImagesDataWriter dataWriter, 
        ImageIndexingTasksService indexingTasksService, 
        ILogger<ImagesControllerBase> logger)
    {
        _dataWriter = dataWriter;
        _indexingTasksService = indexingTasksService;
        _logger = logger;

        _converter = new ImageDataModelConverter();
    }

    protected IActionResult PostData(Data.Models.ImageModel[] models)
    {
        _dataWriter.SaveData(models, out var audit);

        _logger.LogInformation("{audit}", audit.ToString());

        _indexingTasksService.PopulateTasks(audit.Images);

        return Ok();
    }
}
