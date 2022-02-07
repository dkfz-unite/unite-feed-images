using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Images;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories
{
    internal class MriImageRepository : ImageRepositoryBase<MriImageModel>
    {
        public MriImageRepository(DomainDbContext dbContext) : base(dbContext)
        {
        }


        public override Image Find(int donorId, in MriImageModel model)
        {
            var referenceId = model.ReferenceId;

            return _dbContext.Images
                .Include(entity => entity.MriImage)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.MriImage != null &&
                    entity.MriImage.ReferenceId == referenceId
                );
        }


        protected override void Map(in MriImageModel model, ref Image entity)
        {
            base.Map(model, ref entity);

            if (entity.MriImage == null)
            {
                entity.MriImage = new MriImage();
            }

            entity.MriImage.ReferenceId = model.ReferenceId;
            entity.MriImage.WholeTumor = model.WholeTumor;
            entity.MriImage.ContrastEnhancing = model.ContrastEnhancing;
            entity.MriImage.NonContrastEnhancing = model.NonContrastEnhancing;
            entity.MriImage.MedianAdcTumor = model.MedianAdcTumor;
            entity.MriImage.MedianAdcCe = model.MedianAdcCe;
            entity.MriImage.MedianAdcEdema = model.MedianAdcEdema;
            entity.MriImage.MedianCbfTumor = model.MedianCbfTumor;
            entity.MriImage.MedianCbfCe = model.MedianCbfCe;
            entity.MriImage.MedianCbfEdema = model.MedianCbfEdema;
            entity.MriImage.MedianCbvTumor = model.MedianCbvTumor;
            entity.MriImage.MedianCbvCe = model.MedianCbvCe;
            entity.MriImage.MedianCbvEdema = model.MedianCbvEdema;
            entity.MriImage.MedianMttTumor = model.MedianMttTumor;
            entity.MriImage.MedianMttCe = model.MedianMttCe;
            entity.MriImage.MedianMttEdema = model.MedianMttEdema;
        }
    }
}
