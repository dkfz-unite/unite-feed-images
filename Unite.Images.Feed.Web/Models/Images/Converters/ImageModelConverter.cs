namespace Unite.Images.Feed.Web.Models.Images.Converters;

public class ImageModelConverter
{
    public Data.Models.ImageModel Convert(ImageModel source)
    {
        var target = GetImageModel(source);

        target.ScanningDate = source.ScanningDate;
        target.ScanningDay = source.ScanningDay;
        target.Donor = GetDonorModel(source.DonorId);
        target.Analysis = GetAnalysisModel(source.Analysis);

        return target;
    }


    private Data.Models.DonorModel GetDonorModel(string id)
    {
        return new Data.Models.DonorModel
        {
            ReferenceId = id
        };
    }

    private static Data.Models.ImageModel GetImageModel(ImageModel source)
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

    private static Data.Models.MriImageModel GetMriImageModel(ImageModel source)
    {
        return new Data.Models.MriImageModel
        {
            ReferenceId = source.Id,
            WholeTumor = source.MriImage.WholeTumor,
            ContrastEnhancing = source.MriImage.ContrastEnhancing,
            NonContrastEnhancing = source.MriImage.NonContrastEnhancing,
            MedianAdcTumor = source.MriImage.MedianAdcTumor,
            MedianAdcCe = source.MriImage.MedianAdcCe,
            MedianAdcEdema = source.MriImage.MedianAdcEdema,
            MedianCbfTumor = source.MriImage.MedianCbfTumor,
            MedianCbfCe = source.MriImage.MedianCbfCe,
            MedianCbfEdema = source.MriImage.MedianCbfEdema,
            MedianCbvTumor = source.MriImage.MedianCbvTumor,
            MedianCbvCe = source.MriImage.MedianCbvCe,
            MedianCbvEdema = source.MriImage.MedianCbvEdema,
            MedianMttTumor = source.MriImage.MedianMttTumor,
            MedianMttCe = source.MriImage.MedianMttCe,
            MedianMttEdema = source.MriImage.MedianMttEdema
        };
    }

    private static Data.Models.AnalysisModel GetAnalysisModel(AnalysisModel source)
    {
        if (source == null)
        {
            return null;
        }

        return new Data.Models.AnalysisModel
        {
            ReferenceId = source.Id,
            Type = source.Type,
            Date = source.Date,
            Day = source.Day,
            Parameters = source.Parameters,
            RadiomicsFeatures = source.Features?.Select(GetRadiomicsFeatureModel)
        };
    }

    private static Data.Models.ParameterModel GetParameterModel(KeyValuePair<string, string> source)
    {
        return new Data.Models.ParameterModel
        {
            Name = source.Key,
            Value = source.Value
        };
    }

    private static Data.Models.RadiomicsFeatureModel GetRadiomicsFeatureModel(KeyValuePair<string, string> source)
    {
        return new Data.Models.RadiomicsFeatureModel
        {
            Name = source.Key,
            Value = source.Value
        };
    }
}
