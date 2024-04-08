using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Models.Converters;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Indices.Services;

namespace Unite.Images.Feed.Web.Controllers;

public class ImagesControllerBase : Controller
{
    protected readonly ImagesDataWriter _dataWriter;
    protected readonly ImagesDataRemover _dataRemover;
    protected readonly ImageIndexRemovalService _indexRemover;
    protected readonly ImageIndexingTasksService _indexingTasksService;
    protected readonly ILogger _logger;

    protected readonly ImageDataModelsConverter _converter = new();

    public ImagesControllerBase(
        ImagesDataWriter dataWriter,
        ImagesDataRemover dataRemover,
        ImageIndexRemovalService indexRemover,
        ImageIndexingTasksService indexingTasksService,
        ILogger<ImagesControllerBase> logger)
    {
        _dataWriter = dataWriter;
        _dataRemover = dataRemover;
        _indexRemover = indexRemover;
        _indexingTasksService = indexingTasksService;
        _logger = logger;
    }

    protected IActionResult PostData(Data.Models.ImageModel[] models)
    {
        _dataWriter.SaveData(models, out var audit);

        _logger.LogInformation("{audit}", audit.ToString());

        _indexingTasksService.PopulateTasks(audit.Images);

        return Ok();
    }

    protected IActionResult DeleteData(int id)
    {
        var image = _dataRemover.Find(id);

        if (image != null)
        {
            _indexingTasksService.ChangeStatus(false);
            _indexingTasksService.PopulateTasks([id]);
            _indexRemover.DeleteIndex(id);
            _dataRemover.SaveData(image);
            _indexingTasksService.ChangeStatus(true);

            _logger.LogInformation("Image `{id}` has been deleted", id);

            return Ok();
        }
        else
        {
            _logger.LogWarning("Wrong attempt to delete image `{id}`", id);

            return NotFound();
        }
    }
}
