using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Images.Feed.Data;
using Unite.Images.Feed.Web.Configuration.Options;
using Unite.Images.Feed.Web.Handlers;
using Unite.Images.Feed.Web.Workers;
using Unite.Images.Feed.Web.Models;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Base.Validators;
using Unite.Images.Feed.Web.Models.Validators;
using Unite.Images.Feed.Web.Services;
using Unite.Images.Indices.Services;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;

using RadFeatureModel = Unite.Images.Feed.Web.Models.Radiomics.FeatureModel;
using RadFeatureModelValidator = Unite.Images.Feed.Web.Models.Radiomics.Validators.FeatureModelValidator;

namespace Unite.Images.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        var sqlOptions = new SqlOptions();

        services.AddOptions();
        services.AddDatabase();
        services.AddDatabaseFactory(sqlOptions);
        services.AddRepositories();
        services.AddIndexServices();
        services.AddValidators();

        services.AddTransient<ImagesWriter>();
        services.AddTransient<ImagesRemover>();
        services.AddTransient<AnalysisWriter>();

        services.AddTransient<ImageIndexingTasksService>();
        services.AddTransient<TasksProcessingService>();

        services.AddHostedService<IndexingWorker>();
        services.AddTransient<ImagesIndexingOptions>();
        services.AddTransient<ImagesIndexingHandler>();
        services.AddTransient<ImageIndexCreationService>();
        services.AddTransient<ImageIndexRemovalService>();
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
        services.AddTransient<IValidator<MriImageModel[]>, MriImageModelsValidator>();
        services.AddTransient<IValidator<AnalysisModel<RadFeatureModel>>, AnalysisModelValidator<RadFeatureModel, RadFeatureModelValidator>>();

        return services;
    }
}
