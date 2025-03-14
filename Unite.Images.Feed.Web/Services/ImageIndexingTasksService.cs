using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Images;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Images.Feed.Web.Services;

public class ImageIndexingTasksService : IndexingTaskService<Image, int>
{
    protected override int BucketSize => 1000;

    private readonly ImagesRepository _imagesRepository;


    public ImageIndexingTasksService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _imagesRepository = new ImagesRepository(dbContextFactory);
    }
    

    public override void CreateTasks()
    {
        IterateEntities<Image, int>(image => true, image => image.Id, images =>
        {
            CreateImageIndexingTasks(images);
        });
    }

    public override void CreateTasks(IEnumerable<int> imageIds)
    {
        IterateEntities<Image, int>(image => imageIds.Contains(image.Id), image => image.Id, images =>
        {
            CreateImageIndexingTasks(images);
        });
    }

    public override void PopulateTasks(IEnumerable<int> imageIds)
    {
        IterateEntities<Image, int>(image => imageIds.Contains(image.Id), image => image.Id, images =>
        {
            CreateProjectIndexingTasks(images);
            CreateDonorIndexingTasks(images);
            CreateImageIndexingTasks(images);
            CreateSpecimenIndexingTasks(images);
            CreateGeneIndexingTasks(images);
            CreateVariantIndexingTasks(images);
        });
    }


    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedProjects(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedDonors(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedSpecimens(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedGenes(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSsms(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedVariants<SSM.Variant>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedCnvs(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedVariants<CNV.Variant>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSvs(IEnumerable<int> keys)
    {
        return _imagesRepository.GetRelatedVariants<SV.Variant>(keys).Result;
    }
}
