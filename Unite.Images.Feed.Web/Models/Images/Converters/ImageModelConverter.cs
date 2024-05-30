using Unite.Images.Feed.Web.Models.Base.Converters;

namespace Unite.Images.Feed.Web.Models.Converters;

public class ImageModelConverter
{
    private readonly MriImageModelConverter _mriImageModelsConverter = new();

    public Data.Models.ImageModel Convert(in ImageModel source)
    {
        var target = GetImage(source);

        target.CreationDate = source.CreationDate;
        target.CreationDay = source.CreationDay;
        target.Donor = GetDonor(source.DonorId);

        return target;
    }

    public Data.Models.ImageModel[] Convert(ImageModel[] source)
    {
        return source.Select(model => Convert(model)).ToArray();
    }


    private Data.Models.DonorModel GetDonor(string id)
    {
        return new Data.Models.DonorModel
        {
            ReferenceId = id
        };
    }

    private Data.Models.ImageModel GetImage(ImageModel source)
    {
        if (source.MriImage != null)
            return _mriImageModelsConverter.Convert(source.Id, source.MriImage);
        else
            throw new NotImplementedException("Image type is not supported yet");
    }
}
