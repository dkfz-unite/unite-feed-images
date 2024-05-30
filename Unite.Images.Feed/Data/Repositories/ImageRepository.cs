using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Analysis;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal class ImageRepository
{
    private const string UnsupportedImageType = "Image type is not yet supported";

    private readonly DomainDbContext _dbContext;
    private readonly ImageRepositoryBase<MriImageModel> _mriImageRepository;


    public ImageRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _mriImageRepository = new MriImageRepository(dbContext);
    }


    public Image Find(int id)
    {
        return _dbContext.Set<Image>().FirstOrDefault(entity =>
            entity.Id == id
        );
    }

    public Image Find(ImageModel model)
    {
        if (model is MriImageModel mriImage)
            return _mriImageRepository.Find(mriImage);
        else
            throw new NotImplementedException(UnsupportedImageType);
    }

    public Image Create(ImageModel model)
    {
        if (model is MriImageModel mriImage)
            return _mriImageRepository.Create(mriImage);
        else
            throw new NotImplementedException(UnsupportedImageType);
    }

    public Image FindOrCreate(ImageModel model)
    {
        return Find(model) ?? Create(model);
    }

    public void Update(Image entity, ImageModel model)
    {
        if (model is MriImageModel mriImage)
            _mriImageRepository.Update(entity, mriImage);
        else
            throw new NotImplementedException(UnsupportedImageType);
    }

    public void Delete(Image image)
    {
        var analyses = _dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(entity => entity.Analysis)
            .Where(entity => entity.SpecimenId == image.Id)
            .Select(entity => entity.Analysis)
            .Distinct()
            .ToArray();

        _dbContext.RemoveRange(analyses);
        _dbContext.Remove(image);
        _dbContext.SaveChanges();
    }
}
