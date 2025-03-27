using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Images;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly IIndexService<ImageIndex> _indexService;
    private readonly ImageIndexingTasksService _tasksService;

    public IndexingController(
        IIndexService<ImageIndex> indexService,
        ImageIndexingTasksService tasksService)
    {
        _indexService = indexService;
        _tasksService = tasksService;
    }

    [HttpPost]
    public IActionResult Post()
    {
        _indexService.DeleteIndex().Wait();
        _tasksService.CreateTasks();
        return Ok();
    } 
}
