using Unite.Data.Services;
using Unite.Images.Feed.Data.Models;
using Unite.Images.Feed.Data.Models.Audit;
using Unite.Images.Feed.Data.Repositories;

namespace Unite.Images.Feed.Data;

public class ImageDataWriter : DataWriter<ImageModel, ImagesUploadAudit>
{
    private readonly DonorRepository _donorRepository;
    private readonly ImageRepository _imageRepository;
    private readonly AnalysedImageRepository _analysedImageRepository;
    private readonly FeatureOccurrenceRepository _featureOccurrenceRepository;


    public ImageDataWriter(DomainDbContext dbContext) : base(dbContext)
    {
        _donorRepository = new DonorRepository(dbContext);
        _imageRepository = new ImageRepository(dbContext);
        _analysedImageRepository = new AnalysedImageRepository(dbContext);
        _featureOccurrenceRepository = new FeatureOccurrenceRepository(dbContext);
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

            if (model.Analysis.Features?.Any() == true)
            {
                _featureOccurrenceRepository.CreateOrUpdate(analysedImage.Id, model.Analysis.Features);
            }

            audit.ImagesAnalysed++;
        }


        audit.Images.Add(image.Id);
    }
}
