using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal class MrImageRepository : ImageRepositoryBase<MrImageModel>
{
    public MrImageRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }


    protected override IQueryable<Image> GetQuery()
    {
        return base.GetQuery()
            .Include(image => image.MrImage);
    }

    protected override void Map(MrImageModel model, Image entity)
    {
        base.Map(model, entity);

        if (entity.MrImage == null)
            entity.MrImage = new MrImage();

        entity.MrImage.WholeTumor = model.WholeTumor;
        entity.MrImage.ContrastEnhancing = model.ContrastEnhancing;
        entity.MrImage.NonContrastEnhancing = model.NonContrastEnhancing;
        entity.MrImage.MedianAdcTumor = model.MedianAdcTumor;
        entity.MrImage.MedianAdcCe = model.MedianAdcCe;
        entity.MrImage.MedianAdcEdema = model.MedianAdcEdema;
        entity.MrImage.MedianCbfTumor = model.MedianCbfTumor;
        entity.MrImage.MedianCbfCe = model.MedianCbfCe;
        entity.MrImage.MedianCbfEdema = model.MedianCbfEdema;
        entity.MrImage.MedianCbvTumor = model.MedianCbvTumor;
        entity.MrImage.MedianCbvCe = model.MedianCbvCe;
        entity.MrImage.MedianCbvEdema = model.MedianCbvEdema;
        entity.MrImage.MedianMttTumor = model.MedianMttTumor;
        entity.MrImage.MedianMttCe = model.MedianMttCe;
        entity.MrImage.MedianMttEdema = model.MedianMttEdema;
    }
}
