using System;
using Microsoft.AspNetCore.Mvc;

namespace Unite.Radiology.Feed.Web.Controllers
{
    [Route("api/")]
    public class DefaultController : Controller
    {
        public IActionResult Get()
        {
            var date = DateTime.UtcNow;

            return Json(date);
        }
    }
}
