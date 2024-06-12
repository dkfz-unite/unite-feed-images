using Unite.Data.Context;
using Unite.Data.Entities.Images.Analysis.Radiomics;
using Unite.Images.Feed.Data.Models.Radiomics;

namespace Unite.Images.Feed.Data.Repositories.Radiomics;

public class FeatureEntryRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly FeatureRepository _featureRepository;


    public FeatureEntryRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _featureRepository = new FeatureRepository(dbContext);
    }


    public FeatureEntry Find(int sampleId, FeatureModel model)
    {
        var feature = _featureRepository.Find(model.Name);

        if (feature == null)
            return null;

        return Find(sampleId, feature.Id);
    }

    public IEnumerable<FeatureEntry> Recreate(int sampleId, IEnumerable<FeatureModel> models)
    {
        RemoveAll(sampleId);

        var entities = new List<FeatureEntry>();

        foreach (var model in models)
        {
            var entity = Find(sampleId, model);

            if (entity == null)
            {
                var featureId = _featureRepository.FindOrCreate(model.Name).Id;

                entity = new FeatureEntry()
                {
                    SampleId = sampleId,
                    EntityId = featureId,
                    Value = model.Value
                };

                entities.Add(entity);
            }
        }

        if (entities.Any())
        {
            _dbContext.AddRange(entities);
            _dbContext.SaveChanges();
        }

        return entities;
    }


    private void RemoveAll(int sampleId)
    {
        var entitiesToRemove = _dbContext.Set<FeatureEntry>()
            .Where(entity => entity.SampleId == sampleId)
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }

    private FeatureEntry Find(int sampleId, int featureId)
    {
        return _dbContext.Set<FeatureEntry>()
            .FirstOrDefault(entity =>
                entity.SampleId == sampleId &&
                entity.EntityId == featureId
        );
    }
}
