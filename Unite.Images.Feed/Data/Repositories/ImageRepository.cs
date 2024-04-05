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

    public void Delete(int id)
    {
        var image = _dbContext.Set<Image>()
            .AsNoTracking()
            .FirstOrDefault(entity => entity.Id == id);

        if (image == null)
        {
            throw new NotFoundException($"Image with id '{id}' was not found");
        }

        var analyses = _dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Where(entity => entity.TargetSampleId == id)
            .ToArray();

        _dbContext.Remove(image);
        _dbContext.RemoveRange(analyses);
        _dbContext.SaveChanges();
    }
}
