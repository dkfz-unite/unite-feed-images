using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
[Authorize]
public class IndexingController : Controller
{
    private readonly ImageIndexingTasksService _indexingTasksService;


    public IndexingController(ImageIndexingTasksService indexingTasksService)
    {
        _indexingTasksService = indexingTasksService;
    }


    [HttpPost]
    public IActionResult Images()
    {
        _indexingTasksService.CreateTasks();

        return Ok();
    }
}
