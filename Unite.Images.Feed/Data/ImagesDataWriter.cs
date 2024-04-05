using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Images.Feed.Data.Models;
using Unite.Images.Feed.Data.Repositories;

namespace Unite.Images.Feed.Data;

public class ImagesDataWriter : DataWriter<ImageModel, ImagesDataUploadAudit>
{
    private DonorRepository _donorRepository;
    private ImageRepository _imageRepository;
    private AnalysedSampleRepository _analysedImageRepository;
    private RadiomicsFeatureEntryRepository _featureOccurrenceRepository;


    public ImagesDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected override void Initialize(DomainDbContext dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _imageRepository = new ImageRepository(dbContext);
        _analysedImageRepository = new AnalysedSampleRepository(dbContext);
        _featureOccurrenceRepository = new RadiomicsFeatureEntryRepository(dbContext);
    }

    protected override void ProcessModel(ImageModel model, ref ImagesDataUploadAudit audit)
    {
        var donor = _donorRepository.Find(model.Donor);

        if (donor == null)
        {
            donor = _donorRepository.Create(model.Donor);

            audit.DonorsCreated++;
        }


        var image = _imageRepository.Find(donor.Id, model);

        if (image == null)
        {
            image = _imageRepository.Create(donor.Id, model);

            audit.ImagesCreated++;
        }
        else
        {
            _imageRepository.Update(ref image, model);

            audit.ImagesUpdated++;
        }


        if (model.Analysis != null)
        {
            var analysedImage = _analysedImageRepository.CreateOrUpdate(image.Id, model);

            if (model.Analysis.RadiomicsFeatures?.Any() == true)
            {
                _featureOccurrenceRepository.CreateOrUpdate(analysedImage.Id, model.Analysis.RadiomicsFeatures);
            }

            audit.ImagesAnalysed++;
        }


        audit.Images.Add(image.Id);
    }
}
