using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Base.Converters;

namespace Unite.Images.Feed.Web.Models.Converters;

public class ImageDataModelsConverter
{
    private readonly AnalysisModelsConverter _analysisModelsConverter = new();
    private readonly MriImageModelsConverter _mriImageModelsConverter = new();

    public Data.Models.ImageModel Convert(in ImageDataModel source)
    {
        var target = GetImageModel(source);

        target.ScanningDate = source.ScanningDate;
        target.ScanningDay = source.ScanningDay;
        target.Donor = GetDonorModel(source.DonorId);
        target.Analysis = GetAnalysisModel(source.Analysis);

        return target;
    }

    public Data.Models.ImageModel[] Convert(ImageDataModel[] source)
    {
        return source.Select(model => Convert(model)).ToArray();
    }


    private Data.Models.DonorModel GetDonorModel(string id)
    {
        return new Data.Models.DonorModel
        {
            ReferenceId = id
        };
    }

    private Data.Models.ImageModel GetImageModel(ImageDataModel source)
    {
        if (source.MriImage != null)
            return _mriImageModelsConverter.Convert(source.Id, source.MriImage);
        else
            throw new NotImplementedException("Image type is not supported yet");
    }

    private Data.Models.AnalysisModel GetAnalysisModel(AnalysisModel source)
    {
        return _analysisModelsConverter.Convert(source);
    }
}
