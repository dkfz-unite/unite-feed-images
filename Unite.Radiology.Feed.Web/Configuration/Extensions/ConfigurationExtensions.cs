using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Indices.Services.Configuration.Options;
using Unite.Radiology.Feed.Web.Configuration.Options;
using Unite.Radiology.Feed.Web.Handlers;
using Unite.Radiology.Feed.Web.HostedServices;
using Unite.Radiology.Feed.Web.Models.Validation;
using Unite.Radiology.Feed.Web.Services;

namespace Unite.Radiology.Feed.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void Configure(this IServiceCollection services)
        {
            AddOptions(services);
            AddDatabases(services);
            AddValidation(services);
            AddServices(services);
            AddHostedServices(services);
        }


        private static void AddOptions(IServiceCollection services)
        {
            services.AddTransient<ISqlOptions, SqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
            services.AddTransient<IndexingOptions>();
        }

        private static void AddDatabases(IServiceCollection services)
        {
            services.AddTransient<UniteDbContext>();
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<IValidationService, ValidationService>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<TaskProcessingService>();
        }

        private static void AddHostedServices(IServiceCollection services)
        {
            services.AddHostedService<IndexingHostedService>();
            services.AddTransient<IndexingHandler>();
        }
    }
}
