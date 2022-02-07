using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Images.Features;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories
{
    public class ImageSampleRepository
    {
        private readonly DomainDbContext _dbContext;


        public ImageSampleRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Sample Find(int imageId, in ImageModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Analysis.ReferenceId))
            {
                var referenceId = model.Analysis.ReferenceId;

                return _dbContext.Set<Sample>()
                    .Include(entity => entity.Analysis)
                    .FirstOrDefault(entity =>
                        entity.ImageId == imageId &&
                        entity.Analysis.ReferenceId == referenceId
                    );
            }
            else
            {
                var analysisType = model.Analysis.Type;

                return _dbContext.Set<Sample>()
                    .Include(entity => entity.Analysis)
                    .FirstOrDefault(entity =>
                        entity.ImageId == imageId &&
                        entity.Analysis.TypeId == analysisType
                    );
            }
        }

        public Sample Create(int imageId, in ImageModel model)
        {
            var entity = new Sample()
            {
                ImageId = imageId
            };

            Map(model, ref entity);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Update(ref Sample entity, in ImageModel model)
        {
            Map(model, ref entity);

            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }

        public Sample CreateOrUpdate(int imageId, in ImageModel model)
        {
            var analysedImage = Find(imageId, model);

            if (analysedImage == null)
            {
                analysedImage = Create(imageId, model);
            }
            else
            {
                Update(ref analysedImage, model);
            }

            return analysedImage;
        }


        private void Map(in ImageModel model, ref Sample entity)
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
