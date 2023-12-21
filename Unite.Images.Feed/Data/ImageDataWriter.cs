using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Images.Feed.Data.Models;
using Unite.Images.Feed.Data.Models.Audit;
using Unite.Images.Feed.Data.Repositories;

namespace Unite.Images.Feed.Data;

public class ImageDataWriter : DataWriter<ImageModel, ImagesUploadAudit>
{
    private readonly DonorRepository _donorRepository;
    private readonly ImageRepository _imageRepository;
    private readonly AnalysedSampleRepository _analysedImageRepository;
    private readonly RadiomicsFeatureEntryRepository _featureOccurrenceRepository;


    public ImageDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        
        _donorRepository = new DonorRepository(dbContext);
        _imageRepository = new ImageRepository(dbContext);
        _analysedImageRepository = new AnalysedSampleRepository(dbContext);
        _featureOccurrenceRepository = new RadiomicsFeatureEntryRepository(dbContext);
    }


    protected override void ProcessModel(ImageModel model, ref ImagesUploadAudit audit)
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
