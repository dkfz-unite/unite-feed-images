using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Tasks;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Images.Feed.Web.Services;

public class ImageIndexingTasksService : IndexingTaskService<Image, int>
{
    protected override int BucketSize => 1000;

    public ImageIndexingTasksService(DomainDbContext dbContext) : base(dbContext)
    {
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
            CreateDonorIndexingTasks(images);
            CreateImageIndexingTasks(images);
            CreateSpecimenIndexingTasks(images);
            CreateGeneIndexingTasks(images);
            CreateVariantIndexingTasks(images);
        });
    }


    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        var donorIds = _dbContext.Set<Image>()
            .Where(image => keys.Contains(image.Id))
            .Select(image => image.DonorId)
            .Distinct()
            .ToArray();

        return donorIds;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        var donorIds = _dbContext.Set<Image>()
            .Where(image => keys.Contains(image.Id))
            .Select(image => image.DonorId)
            .Distinct()
            .ToArray();

        var specimenIds = _dbContext.Set<Specimen>()
            .Where(specimen => donorIds.Contains(specimen.DonorId))
            .Where(specimen => specimen.Tissue != null && specimen.Tissue.TypeId == TissueType.Tumor)
            .Select(specimen => specimen.Id)
            .Distinct()
            .ToArray();

        return specimenIds;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        var specimenIds = LoadRelatedSpecimens(keys).ToArray();

        var ssmAffectedGeneIds = _dbContext.Set<SSM.VariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .SelectMany(occurrence => occurrence.Variant.AffectedTranscripts)
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        var cnvAffectedGeneIds = _dbContext.Set<CNV.VariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .SelectMany(occurrence => occurrence.Variant.AffectedTranscripts)
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        var svAffectedGeneIds = _dbContext.Set<SV.VariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .SelectMany(occurrence => occurrence.Variant.AffectedTranscripts)
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        var geneIds = ssmAffectedGeneIds.Union(cnvAffectedGeneIds).Union(svAffectedGeneIds).ToArray();

        return geneIds;
    }

    protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<int> keys)
    {
        var specimenIds = LoadRelatedSpecimens(keys).ToArray();

        var variantIds = _dbContext.Set<SSM.VariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return variantIds;
    }

    protected override IEnumerable<long> LoadRelatedCopyNumberVariants(IEnumerable<int> keys)
    {
        var specimenIds = LoadRelatedSpecimens(keys).ToArray();

        var variantIds = _dbContext.Set<CNV.VariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return variantIds;
    }

    protected override IEnumerable<long> LoadRelatedStructuralVariants(IEnumerable<int> keys)
    {
        var specimenIds = LoadRelatedSpecimens(keys).ToArray();

        var variantIds = _dbContext.Set<SV.VariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return variantIds;
    }
}
