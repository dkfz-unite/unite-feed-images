using Unite.Data.Context;
using Unite.Data.Entities.Images.Analysis.Radiomics;

namespace Unite.Images.Feed.Data.Repositories.Radiomics;

public class FeatureRepository
{
    private readonly DomainDbContext _dbContext;


    public FeatureRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Feature Find(string name)
    {
        return _dbContext.Set<Feature>().FirstOrDefault(entity => 
            entity.Name == name
        );
    }

    public Feature Create(string name)
    {
        var entity = new Feature() { Name = name };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public Feature FindOrCreate(string name)
    {
        return Find(name) ?? Create(name);
    }

    public virtual IEnumerable<Feature> CreateMissing(IEnumerable<string> names)
    {
        var entitiesToAdd = new List<Feature>();

        foreach (var name in names)
        {
            var entity = Find(name);

            if (entity == null)
            {
                entity = new Feature() { Name = name }; 

                entitiesToAdd.Add(entity);
            }
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd.ToArray();
    }
}
