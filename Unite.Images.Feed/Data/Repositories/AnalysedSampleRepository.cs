using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images.Analysis;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

public class AnalysedSampleRepository
{
    private readonly DomainDbContext _dbContext;


    public AnalysedSampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public AnalysedSample Find(int imageId, in ImageModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Analysis.ReferenceId))
        {
            var referenceId = model.Analysis.ReferenceId;

            return _dbContext.Set<AnalysedSample>()
                .Include(entity => entity.Analysis)
                .FirstOrDefault(entity =>
                    entity.TargetSampleId == imageId &&
                    entity.Analysis.ReferenceId == referenceId
                );
        }
        else
        {
            var analysisType = model.Analysis.Type;
            var analysisDate = model.Analysis.Date;

            return _dbContext.Set<AnalysedSample>()
                .Include(entity => entity.Analysis)
                .FirstOrDefault(entity =>
                    entity.TargetSampleId == imageId &&
                    entity.Analysis.ReferenceId == null &&
                    entity.Analysis.TypeId == analysisType &&
                    entity.Analysis.Date == analysisDate
                );
        }
    }

    public AnalysedSample Create(int imageId, in ImageModel model)
    {
        var entity = new AnalysedSample()
        {
            TargetSampleId = imageId
        };

        Map(model, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public void Update(ref AnalysedSample entity, in ImageModel model)
    {
        Map(model, ref entity);

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }

    public AnalysedSample CreateOrUpdate(int imageId, in ImageModel model)
    {
        var entity = Find(imageId, model);

        if (entity == null)
        {
            entity = Create(imageId, model);
        }
        else
        {
            Update(ref entity, model);
        }

        return entity;
    }


    private static void Map(in ImageModel model, ref AnalysedSample entity)
    {
        if (entity.Analysis == null)
        {
            entity.Analysis = new Analysis();
        }

        entity.Analysis.ReferenceId = model.Analysis.ReferenceId;
        entity.Analysis.TypeId = model.Analysis.Type;
        entity.Analysis.Date = model.Analysis.Date;
        entity.Analysis.Day = model.Analysis.Day;
        entity.Analysis.Parameters = model.Analysis.Parameters;
    }
}
