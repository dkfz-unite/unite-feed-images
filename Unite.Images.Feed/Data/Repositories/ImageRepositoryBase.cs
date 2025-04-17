using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal abstract class ImageRepositoryBase<TModel> where TModel : ImageModel
{
    protected readonly DomainDbContext _dbContext;
    protected readonly DonorRepository _donorRepository;


    public ImageRepositoryBase(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _donorRepository = new DonorRepository(dbContext);
    }


    public virtual Image Find(TModel model)
    {
        var type = GetImageType(model);
        var donor = _donorRepository.Find(model.Donor);

        if (donor == null)
            return null;

        return GetQuery().FirstOrDefault(entity =>
            entity.DonorId == donor.Id &&
            entity.ReferenceId == model.ReferenceId &&
            entity.TypeId == type
        );
    }

    public virtual Image Create(TModel model)
    {
        var type = GetImageType(model);
        var donor = _donorRepository.FindOrCreate(model.Donor);

        var entity = new Image()
        {
            DonorId = donor.Id,
            ReferenceId = model.ReferenceId,
            TypeId = type
        };

        Map(model, entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public virtual Image FindOrCreate(TModel model)
    {
        return Find(model) ?? Create(model);
    }

    public virtual void Update(Image entity, TModel model)
    {
        Map(model, entity);

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }


    protected virtual IQueryable<Image> GetQuery()
    {
        return _dbContext.Set<Image>();
    }

    protected virtual void Map(TModel model, Image entity)
    {
        entity.CreationDate = model.CreationDate;
        entity.CreationDay = model.CreationDay;
    }


    private static ImageType GetImageType(ImageModel model)
    {
        if (model is MrImageModel)
            return ImageType.MR;
        else
            throw new NotSupportedException("Image type is not supported");
    }
}
