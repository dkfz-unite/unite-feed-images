using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Data.Exceptions;
using Unite.Images.Feed.Web.Models.Converters;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

public class ImagesControllerBase : Controller
{
    protected readonly ImagesDataWriter _dataWriter;
    protected readonly ImagesDataRemover _dataRemover;
    protected readonly ImageIndexingTasksService _indexingTasksService;
    protected readonly ILogger _logger;

    protected readonly ImageDataModelsConverter _converter = new();

    public ImagesControllerBase(
        ImagesDataWriter dataWriter,
        ImagesDataRemover dataRemover,
        ImageIndexingTasksService indexingTasksService,
        ILogger<ImagesControllerBase> logger)
    {
        _dataWriter = dataWriter;
        _dataRemover = dataRemover;
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
        try
        {
            _indexingTasksService.ChangeStatus(false);

            _indexingTasksService.PopulateTasks([id]);

            _dataRemover.SaveData(id);

            _logger.LogInformation("Deleted specimen `{id}`", id);

            return Ok();
        }
        catch (NotFoundException exception)
        {
            _logger.LogWarning("{error}", exception.Message);

            return BadRequest(exception.Message);
        }
        finally
        {
            _indexingTasksService.ChangeStatus(true);
        }
    }
}
