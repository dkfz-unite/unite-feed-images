using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal class MriImageRepository : ImageRepositoryBase<MriImageModel>
{
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
            entity.MriImage = new MriImage();

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
