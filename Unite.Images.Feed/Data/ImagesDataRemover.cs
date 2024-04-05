using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Images.Feed.Data.Repositories;

namespace Unite.Images.Feed.Data;

public class ImagesDataRemover : DataWriter<int>
{
    private ImageRepository _imageRepository;

    public ImagesDataRemover(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _imageRepository = new ImageRepository(dbContext);
    }

    protected override void ProcessModel(int model)
    {
        _imageRepository.Delete(model);
    }
}
