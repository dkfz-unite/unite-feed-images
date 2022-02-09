using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Images.Features;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories
{
    public class AnalysedImageRepository
    {
        private readonly DomainDbContext _dbContext;


        public AnalysedImageRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public AnalysedImage Find(int imageId, in ImageModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Analysis.ReferenceId))
            {
                var referenceId = model.Analysis.ReferenceId;

                return _dbContext.Set<AnalysedImage>()
                    .Include(entity => entity.Analysis)
                    .FirstOrDefault(entity =>
                        entity.ImageId == imageId &&
                        entity.Analysis.ReferenceId == referenceId
                    );
            }
            else
            {
                var analysisType = model.Analysis.Type;

                return _dbContext.Set<AnalysedImage>()
                    .Include(entity => entity.Analysis)
                    .FirstOrDefault(entity =>
                        entity.ImageId == imageId &&
                        entity.Analysis.ReferenceId == null &&
                        entity.Analysis.TypeId == analysisType
                    );
            }
        }

        public AnalysedImage Create(int imageId, in ImageModel model)
        {
            var entity = new AnalysedImage()
            {
                ImageId = imageId
            };

            Map(model, ref entity);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Update(ref AnalysedImage entity, in ImageModel model)
        {
            Map(model, ref entity);

            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }

        public AnalysedImage CreateOrUpdate(int imageId, in ImageModel model)
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


        private void Map(in ImageModel model, ref AnalysedImage entity)
        {
            if (entity.Analysis == null)
            {
                entity.Analysis = new Analysis();
            }

            entity.Analysis.ReferenceId = model.Analysis.ReferenceId;
            entity.Analysis.TypeId = model.Analysis.Type;
        }
    }
}
