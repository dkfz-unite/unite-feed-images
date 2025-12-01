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
    public async Task<IActionResult> Post()
    {
        await DeleteIndex(_indexService.DeleteIndex());

        _tasksService.CreateTasks();
        
        return Ok();
    } 

    private static async Task DeleteIndex(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // Ignore errors
        }
    }
}
