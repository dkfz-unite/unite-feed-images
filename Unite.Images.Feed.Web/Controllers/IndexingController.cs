using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly ImageIndexingTasksService _tasksService;

    public IndexingController(
        ImageIndexingTasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpPost()]
    public IActionResult Post()
    {
        _tasksService.CreateTasks();
        return Ok();
    } 
}
