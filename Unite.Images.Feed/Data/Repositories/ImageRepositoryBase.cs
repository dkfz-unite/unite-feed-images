using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal abstract class ImageRepositoryBase<TModel> where TModel : ImageModel
{
    protected readonly DomainDbContext _dbContext;


    public ImageRepositoryBase(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public abstract Image Find(int donorId, in TModel model);

    public virtual Image Create(int donorId, in TModel model)
    {
        var entity = new Image()
        {
            DonorId = donorId,
            ReferenceId = model.ReferenceId,
        };

        Map(model, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public virtual void Update(ref Image entity, in TModel model)
    {
        Map(model, ref entity);

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }


    protected virtual void Map(in TModel model, ref Image entity)
    {
        entity.CreationDate = model.ScanningDate;
        entity.CreationDay = model.ScanningDay;
    }
}
