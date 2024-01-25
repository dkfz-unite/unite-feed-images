using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Constants;
using Unite.Images.Feed.Web.Models;
using Unite.Images.Feed.Web.Services;

namespace Unite.Images.Feed.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.Data.Writer)]
public class ImagesController : ImagesControllerBase
{
    public ImagesController(
        ImagesDataWriter dataWriter,
        ImageIndexingTasksService indexingTasksService,
        ILogger<ImagesController> logger) : base(dataWriter, indexingTasksService, logger)
    {
    }

    [HttpPost]
    [Consumes("application/json")]
    public IActionResult Post([FromBody]ImageDataModel[] models)
    {
        var dataModels = _converter.Convert(models);

        return PostData(dataModels);
    }
}
