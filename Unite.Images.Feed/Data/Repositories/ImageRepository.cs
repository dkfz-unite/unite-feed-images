using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Analysis;
using Unite.Images.Feed.Data.Exceptions;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

internal class ImageRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly ImageRepositoryBase<MriImageModel> _mriImageRepository;


    public ImageRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _mriImageRepository = new MriImageRepository(dbContext);
    }


    public Image Find(int id)
    {
        return _dbContext.Set<Image>()
            .FirstOrDefault(entity =>
                entity.Id == id
            );
    }

    public Image Find(int donorId, ImageModel model)
    {
        if (model is MriImageModel mriImage)
        {
            return _mriImageRepository.Find(donorId, mriImage);
        }
        else
        {
            throw new NotImplementedException("Image type is not yet supported");
        }
    }

    public Image Create(int donorId, ImageModel model)
    {
        if (model is MriImageModel mriImage)
        {
            return _mriImageRepository.Create(donorId, mriImage);
        }
        else
        {
            throw new NotImplementedException("Image type is not yet supported");
        }
    }

    public void Update(ref Image image, in ImageModel imageModel)
    {
        if (imageModel is MriImageModel mriImage)
        {
            _mriImageRepository.Update(ref image, mriImage);
        }
        else
        {
            throw new NotImplementedException("Image type is not yet supported");
        }
    }

    public void Delete(Image image)
    {
        var analyses = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(entity => entity.Analysis)
            .Where(entity => entity.TargetSampleId == image.Id)
            .Select(entity => entity.Analysis)
            .Distinct()
            .ToArray();

        _dbContext.Remove(image);
        _dbContext.RemoveRange(analyses);
        _dbContext.SaveChanges();
    }
}
