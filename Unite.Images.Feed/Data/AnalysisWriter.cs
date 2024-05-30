using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Essentials.Extensions;
using Unite.Images.Feed.Data.Models;
using Unite.Images.Feed.Data.Models.Radiomics;

namespace Unite.Images.Feed.Data;

public class AnalysisWriter : DataWriter<SampleModel, AnalysisWriteAudit>
{
    private Repositories.SampleRepository _sampleRepository;
    private Repositories.Radiomics.FeatureRepository _radFeatureRepository;
    private Repositories.Radiomics.FeatureEntryRepository _radFeatureEntryRepository;


    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new Repositories.SampleRepository(dbContext);
        _radFeatureRepository = new Repositories.Radiomics.FeatureRepository(dbContext);
        _radFeatureEntryRepository = new Repositories.Radiomics.FeatureEntryRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        var sample = _sampleRepository.FindOrCreate(model);

        if (model.RadFeatures.IsNotEmpty())
            WriteRadFeatures(sample.Id, model.RadFeatures, ref audit);
    }

    private void WriteRadFeatures(int sampleId, IEnumerable<FeatureModel> featureModels, ref AnalysisWriteAudit audit)
    {
        var names = featureModels.Select(model => model.Name);
        var features = _radFeatureRepository.CreateMissing(names);
        audit.RadFeatures.AddRange(features.Select(feature => feature.Id));
        audit.RadFeaturesCreated += features.Count();

        var featureEntries = _radFeatureEntryRepository.Recreate(sampleId, featureModels);
        audit.RadFeatureEntries.AddRange(featureEntries.Select(entry => entry.EntityId));
        audit.RadFeaturesAssociated += featureEntries.Count();
    }
}
