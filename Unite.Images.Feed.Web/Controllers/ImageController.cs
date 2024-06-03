using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Indices.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/image")]
[Authorize(Policy = Policies.Data.Writer)]
public class ImageController : Controller
{
    protected readonly ImagesRemover _dataRemover;
    protected readonly ImageIndexRemovalService _indexRemover;
    protected readonly ImageIndexingTasksService _tasksService;
    protected readonly ILogger _logger;


    public ImageController(
        ImagesRemover dataRemover,
        ImageIndexRemovalService indexRemover,
        ImageIndexingTasksService tasksService,
        ILogger<ImageController> logger)
    {
        _dataRemover = dataRemover;
        _indexRemover = indexRemover;
        _tasksService = tasksService;
        _logger = logger;
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var image = _dataRemover.Find(id);

        if (image != null)
        {
            _tasksService.ChangeStatus(false);
            _tasksService.PopulateTasks([id]);
            _indexRemover.DeleteIndex(id);
            _dataRemover.SaveData(image);
            _tasksService.ChangeStatus(true);

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
