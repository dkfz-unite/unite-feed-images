using System.Linq;
using Unite.Data.Entities.Images.Features;
using Unite.Data.Services;

namespace Unite.Images.Feed.Data.Repositories
{
    public class FeatureRepository
    {
        private readonly DomainDbContext _dbContext;


        public FeatureRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Feature FindOrCreate(string name)
        {
            var entity = _dbContext.Set<Feature>().FirstOrDefault(entity => entity.Name == name);

            if (entity == null)
            {
                entity = new Feature()
                {
                    Name = name
                };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }
    }
}
