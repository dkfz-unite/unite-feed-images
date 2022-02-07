using Microsoft.AspNetCore.Mvc;
using Unite.Images.Feed.Web.Configuration.Filters;

namespace Unite.Images.Feed.Web.Configuration.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static void AddMvcOptions(this MvcOptions options)
        {
            options.Filters.Add(typeof(DefaultActionFilter));
            options.Filters.Add(typeof(DefaultExceptionFilter));
        }
    }
}
