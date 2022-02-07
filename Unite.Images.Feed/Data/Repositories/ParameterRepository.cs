using System.Linq;
using Unite.Data.Entities.Images.Features;
using Unite.Data.Services;

namespace Unite.Images.Feed.Data.Repositories
{
    public class ParameterRepository
    {
        private readonly DomainDbContext _dbContext;


        public ParameterRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public AnalysisParameter FindOrCreate(string name)
        {
            var entity = _dbContext.Set<AnalysisParameter>().FirstOrDefault(entity => entity.Name == name);

            if (entity == null)
            {
                entity = new AnalysisParameter()
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
