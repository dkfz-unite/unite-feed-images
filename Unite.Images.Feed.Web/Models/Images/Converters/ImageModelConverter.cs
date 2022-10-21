namespace Unite.Images.Feed.Web.Models.Images.Converters;

public class ImageModelConverter
{
    public Data.Models.ImageModel Convert(ImageModel source)
    {
        var target = GetImageModel(source);

        target.ScanningDate = FromDateTime(source.ScanningDate);
        target.ScanningDay = source.ScanningDay;
        target.Donor = GetDonorModel(source.DonorId);
        target.Analysis = GetAnalysisModel(source.Analysis);

        return target;
    }


    private Data.Models.DonorModel GetDonorModel(string id)
    {
        var target = new Data.Models.DonorModel();

        target.ReferenceId = id;

        return target;
    }

    private Data.Models.ImageModel GetImageModel(ImageModel source)
    {
        if (source.MriImage != null)
        {
            return GetMriImageModel(source);
        }
        else if (source.CtImage != null)
        {
            throw new NotImplementedException("CT images are not supported yet");
        }
        else
        {
            throw new NotImplementedException("Image type is not supported yet");
        }
    }

    private Data.Models.MriImageModel GetMriImageModel(ImageModel source)
    {
        var target = new Data.Models.MriImageModel();

        target.ReferenceId = source.Id;
        target.WholeTumor = source.MriImage.WholeTumor;
        target.ContrastEnhancing = source.MriImage.ContrastEnhancing;
        target.NonContrastEnhancing = source.MriImage.NonContrastEnhancing;
        target.MedianAdcTumor = source.MriImage.MedianAdcTumor;
        target.MedianAdcCe = source.MriImage.MedianAdcCe;
        target.MedianAdcEdema = source.MriImage.MedianAdcEdema;
        target.MedianCbfTumor = source.MriImage.MedianCbfTumor;
        target.MedianCbfCe = source.MriImage.MedianCbfCe;
        target.MedianCbfEdema = source.MriImage.MedianCbfEdema;
        target.MedianCbvTumor = source.MriImage.MedianCbvTumor;
        target.MedianCbvCe = source.MriImage.MedianCbvCe;
        target.MedianCbvEdema = source.MriImage.MedianCbvEdema;
        target.MedianMttTumor = source.MriImage.MedianMttTumor;
        target.MedianMttCe = source.MriImage.MedianMttCe;
        target.MedianMttEdema = source.MriImage.MedianMttEdema;

        return target;
    }

    private Data.Models.AnalysisModel GetAnalysisModel(AnalysisModel source)
    {
        if (source == null)
        {
            return null;
        }

        var target = new Data.Models.AnalysisModel();

        target.ReferenceId = source.Id;
        target.Type = source.Type;
        target.Date = FromDateTime(source.Date);
        target.Parameters = source.Parameters;
        target.Features = source.Features?.Select(feature => GetFeatureModel(feature));

        return target;
    }

    private Data.Models.ParameterModel GetParameterModel(KeyValuePair<string, string> source)
    {
        var target = new Data.Models.ParameterModel();

        target.Name = source.Key;
        target.Value = source.Value;

        return target;
    }

    private Data.Models.FeatureModel GetFeatureModel(KeyValuePair<string, string> source)
    {
        var target = new Data.Models.FeatureModel();

        target.Name = source.Key;
        target.Value = source.Value;

        return target;
    }


    private static DateOnly? FromDateTime(DateTime? date)
    {
        return date != null
             ? DateOnly.FromDateTime(date.Value)
             : null;
    }
}
