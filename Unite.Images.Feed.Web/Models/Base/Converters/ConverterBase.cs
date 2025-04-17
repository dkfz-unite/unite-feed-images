using Unite.Data.Entities.Images.Enums;

namespace Unite.Images.Feed.Web.Models.Base.Converters;

public abstract class ConverterBase
{
    protected virtual Data.Models.ImageModel GetImage(string donorId, string imageId, ImageType? imageType)
    {
        if (imageId == null || imageType == null)
            return null;

        Data.Models.ImageModel image;

        if (imageType == ImageType.MR)
            image = new Data.Models.MrImageModel { ReferenceId = imageId };
        else
            throw new NotSupportedException($"Image type '{imageType}' is not supported");

        image.Donor = GetDonor(donorId);

        return image;
    }

    protected virtual Data.Models.DonorModel GetDonor(string donorId)
    {
        return new Data.Models.DonorModel { ReferenceId = donorId };
    }
}
