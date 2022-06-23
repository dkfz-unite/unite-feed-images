using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Data.Repositories;

public class DonorRepository
{
    private readonly DomainDbContext _dbContext;


    public DonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Donor Find(DonorModel model)
    {
        var donor = _dbContext.Donors.FirstOrDefault(entity =>
            entity.ReferenceId == model.ReferenceId
        );

        return donor;
    }

    public Donor Create(DonorModel model)
    {
        var entity = new Donor();

        Map(model, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }


    private void Map(in DonorModel model, ref Donor entity)
    {
        entity.ReferenceId = model.ReferenceId;
    }
}
