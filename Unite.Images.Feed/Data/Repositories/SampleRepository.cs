using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Analysis;
using Unite.Data.Entities.Images.Enums;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

public class SampleRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly ImageRepository _imageRepository;


    public SampleRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _imageRepository = new ImageRepository(dbContext);
    }


    public Sample FindOrCreate(SampleModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Sample Find(SampleModel model)
    {
        var imageType = GetImageType(model.Image);

        return _dbContext.Set<Sample>().AsNoTracking().FirstOrDefault(entity =>
            entity.Specimen.Donor.ReferenceId == model.Image.Donor.ReferenceId &&
            entity.Specimen.ReferenceId == model.Image.ReferenceId &&
            entity.Specimen.TypeId == imageType &&
            entity.Analysis.TypeId == model.Analysis.Type
        );
    }

    public Sample Create(SampleModel model)
    {
        var entity = new Sample()
        {
            Analysis = Create(model.Analysis),
            Specimen = FindOrCreate(model.Image)
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }


    private Analysis Create(AnalysisModel model)
    {
        var entity = new Analysis
        {
            TypeId = model.Type,
            Date = model.Date,
            Day = model.Day,
            Parameters = model.Parameters
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private Image FindOrCreate(ImageModel model)
    {
        return _imageRepository.FindOrCreate(model);
    }


    private static ImageType GetImageType(ImageModel model)
    {
        if (model is MriImageModel)
            return ImageType.MRI;
        else
            throw new ArgumentException("Unknown image type");
    }
}
