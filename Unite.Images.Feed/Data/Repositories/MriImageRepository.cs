using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal class MriImageRepository : ImageRepositoryBase<MriImageModel>
{
    protected override ImageType Type => ImageType.MRI;


    public MriImageRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }


    protected override IQueryable<Image> GetQuery()
    {
        return base.GetQuery()
            .Include(image => image.MriImage);
    }

    protected override void Map(MriImageModel model, Image entity)
    {
        base.Map(model, entity);

        if (entity.MriImage == null)
        {
            entity.MriImage = new()
            {
                WholeTumor = model.WholeTumor,
                ContrastEnhancing = model.ContrastEnhancing,
                NonContrastEnhancing = model.NonContrastEnhancing,
                MedianAdcTumor = model.MedianAdcTumor,
                MedianAdcCe = model.MedianAdcCe,
                MedianAdcEdema = model.MedianAdcEdema,
                MedianCbfTumor = model.MedianCbfTumor,
                MedianCbfCe = model.MedianCbfCe,
                MedianCbfEdema = model.MedianCbfEdema,
                MedianCbvTumor = model.MedianCbvTumor,
                MedianCbvCe = model.MedianCbvCe,
                MedianCbvEdema = model.MedianCbvEdema,
                MedianMttTumor = model.MedianMttTumor,
                MedianMttCe = model.MedianMttCe,
                MedianMttEdema = model.MedianMttEdema
            };
        }
    }
}
