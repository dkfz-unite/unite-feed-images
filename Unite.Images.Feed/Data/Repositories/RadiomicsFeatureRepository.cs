using Unite.Data.Context;
using Unite.Data.Entities.Images.Features;

namespace Unite.Images.Feed.Data.Repositories;

public class RadiomicsFeatureRepository
{
    private readonly DomainDbContext _dbContext;


    public RadiomicsFeatureRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public RadiomicsFeature FindOrCreate(string name)
    {
        var entity = _dbContext.Set<RadiomicsFeature>().FirstOrDefault(entity => entity.Name == name);

        if (entity == null)
        {
            entity = new RadiomicsFeature()
            {
                Name = name
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();
        }

        return entity;
    }
}
