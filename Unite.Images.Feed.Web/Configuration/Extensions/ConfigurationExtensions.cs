using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Options;
using Unite.Images.Feed.Web.Handlers;
using Unite.Images.Feed.Web.HostedServices;
using Unite.Images.Feed.Web.Models;
using Unite.Images.Feed.Web.Models.Validators;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Indices.Entities.Images;

namespace Unite.Images.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        var sqlOptions = new SqlOptions();

        services.AddOptions();
        services.AddDatabase();
        services.AddDatabaseFactory(sqlOptions);
        services.AddIndexServices();
        services.AddValidators();

        services.AddTransient<ImagesDataWriter>();
        services.AddTransient<ImagesDataRemover>();

        services.AddTransient<ImageIndexingTasksService>();
        services.AddTransient<TasksProcessingService>();

        services.AddHostedService<IndexingHostedService>();
        services.AddTransient<ImagesIndexingOptions>();
        services.AddTransient<ImagesIndexingHandler>();
        services.AddTransient<ImageIndexCreationService>();
    }


    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddTransient<ApiOptions>();
        services.AddTransient<ISqlOptions, SqlOptions>();
        services.AddTransient<IElasticOptions, ElasticOptions>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<ImageDataModel[]>, ImageModelsValidator>();

        return services;
    }
}
