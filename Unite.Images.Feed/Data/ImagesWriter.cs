using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Images.Feed.Data.Models;
using Unite.Images.Feed.Data.Repositories;

namespace Unite.Images.Feed.Data;

public class ImagesWriter : DataWriter<ImageModel, ImagesWriteAudit>
{
    private ImageRepository _imageRepository;


    public ImagesWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        Initialize(dbContext);
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _imageRepository = new ImageRepository(dbContext);
    }

    protected override void ProcessModel(ImageModel model, ref ImagesWriteAudit audit)
    {
        var image = _imageRepository.Find(model);

        if (image == null)
        {
            image = _imageRepository.Create(model);
            audit.ImagesCreated++;
        }
        else
        {
            _imageRepository.Update(image, model);
            audit.ImagesUpdated++;
        }

        audit.Images.Add(image.Id);
    }
}
