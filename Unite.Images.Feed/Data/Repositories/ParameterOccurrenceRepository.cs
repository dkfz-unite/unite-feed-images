using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Images.Features;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

public class ParameterOccurrenceRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly ParameterRepository _parameterRepository;


    public ParameterOccurrenceRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _parameterRepository = new ParameterRepository(dbContext);
    }


    public AnalysisParameterOccurrence Find(int analysisId, ParameterModel model)
    {
        return _dbContext.Set<AnalysisParameterOccurrence>()
            .Include(entity => entity.Parameter)
            .FirstOrDefault(entity =>
                entity.AnalysisId == analysisId &&
                entity.Parameter.Name == model.Name
            );
    }

    public IEnumerable<AnalysisParameterOccurrence> CreateOrUpdate(int analysisId, IEnumerable<ParameterModel> models)
    {
        RemoveRedundant(analysisId, models);

        var created = CreateMissing(analysisId, models);

        var updated = UpdateExisting(analysisId, models);

        return Enumerable.Concat(created, updated);
    }

    public IEnumerable<AnalysisParameterOccurrence> CreateMissing(int analysisId, IEnumerable<ParameterModel> models)
    {
        var entitiesToAdd = new List<AnalysisParameterOccurrence>();

        foreach (var model in models)
        {
            var entity = Find(analysisId, model);

            if (entity == null)
            {
                var parameterId = _parameterRepository.FindOrCreate(model.Name).Id;

                entity = new AnalysisParameterOccurrence()
                {
                    AnalysisId = analysisId,
                    ParameterId = parameterId
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

    public IEnumerable<AnalysisParameterOccurrence> UpdateExisting(int analysisId, IEnumerable<ParameterModel> models)
    {
        var entitiesToUpdate = new List<AnalysisParameterOccurrence>();

        foreach (var model in models)
        {
            var entity = Find(analysisId, model);

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

    public void RemoveRedundant(int analysisId, IEnumerable<ParameterModel> models)
    {
        var parameterNames = models.Select(model => model.Name);

        var entitiesToRemove = _dbContext.Set<AnalysisParameterOccurrence>()
            .Include(entity => entity.Parameter)
            .Where(entity => entity.AnalysisId == analysisId && !parameterNames.Contains(entity.Parameter.Name))
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private void Map(in ParameterModel model, ref AnalysisParameterOccurrence entity)
    {
        entity.Value = model.Value;
    }
}
