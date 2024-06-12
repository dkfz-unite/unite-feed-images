using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Data.Entities.Images;
using Unite.Images.Feed.Data.Repositories;

namespace Unite.Images.Feed.Data;

public class ImagesRemover : DataWriter<Image>
{
    private ImageRepository _imageRepository;

    public ImagesRemover(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();

        Initialize(dbContext);
    }


    public Image Find(int id)
    {
        return _imageRepository.Find(id);
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _imageRepository = new ImageRepository(dbContext);
    }

    protected override void ProcessModel(Image image)
    {
        _imageRepository.Delete(image);
    }
}
