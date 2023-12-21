using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images.Features;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

public class RadiomicsFeatureEntryRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly RadiomicsFeatureRepository _featureRepository;


    public RadiomicsFeatureEntryRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _featureRepository = new RadiomicsFeatureRepository(dbContext);
    }


    public RadiomicsFeatureEntry Find(int analysedImageId, RadiomicsFeatureModel model)
    {
        return _dbContext.Set<RadiomicsFeatureEntry>()
            .Include(entity => entity.Entity)
            .FirstOrDefault(entity =>
                entity.AnalysedSampleId == analysedImageId &&
                entity.Entity.Name == model.Name
        );
    }

    public IEnumerable<RadiomicsFeatureEntry> CreateOrUpdate(int analysedImageId, IEnumerable<RadiomicsFeatureModel> models)
    {
        RemoveRedundant(analysedImageId, models);

        var created = CreateMissing(analysedImageId, models);

        var updated = UpdateExisting(analysedImageId, models);

        return Enumerable.Concat(created, updated);
    }

    public IEnumerable<RadiomicsFeatureEntry> CreateMissing(int analysedImageId, IEnumerable<RadiomicsFeatureModel> models)
    {
        var entitiesToAdd = new List<RadiomicsFeatureEntry>();

        foreach (var model in models)
        {
            var entity = Find(analysedImageId, model);

            if (entity == null)
            {
                var featureId = _featureRepository.FindOrCreate(model.Name).Id;

                entity = new RadiomicsFeatureEntry()
                {
                    AnalysedSampleId = analysedImageId,
                    EntityId = featureId
                };

                Map(model, ref entity);

                entitiesToAdd.Add(entity);
            }
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public IEnumerable<RadiomicsFeatureEntry> UpdateExisting(int analysedImageId, IEnumerable<RadiomicsFeatureModel> models)
    {
        var entitiesToUpdate = new List<RadiomicsFeatureEntry>();

        foreach (var model in models)
        {
            var entity = Find(analysedImageId, model);

            if (entity != null)
            {
                Map(model, ref entity);

                entitiesToUpdate.Add(entity);
            }
        }

        if (entitiesToUpdate.Any())
        {
            _dbContext.UpdateRange(entitiesToUpdate);
            _dbContext.SaveChanges();
        }

        return entitiesToUpdate;
    }

    public void RemoveRedundant(int analysedImageId, IEnumerable<RadiomicsFeatureModel> models)
    {
        var featureNames = models.Select(model => model.Name);

        var entitiesToRemove = _dbContext.Set<RadiomicsFeatureEntry>()
            .Include(entity => entity.Entity)
            .Where(entity => entity.AnalysedSampleId == analysedImageId && !featureNames.Contains(entity.Entity.Name))
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private static void Map(in RadiomicsFeatureModel model, ref RadiomicsFeatureEntry entity)
    {
        entity.Value = model.Value;
    }
}
