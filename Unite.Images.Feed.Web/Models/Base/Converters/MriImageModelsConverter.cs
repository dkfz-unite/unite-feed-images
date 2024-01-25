namespace Unite.Images.Feed.Web.Models.Base.Converters;

public class MriImageModelsConverter
{
    public Data.Models.MriImageModel Convert(string referenceId, in MriImageModel model)
    {
        if (model == null)
            return null;

        return new Data.Models.MriImageModel
        {
            ReferenceId = referenceId,
            WholeTumor = model.WholeTumor,
            ContrastEnhancing = model.ContrastEnhancing,
            NonContrastEnhancing = model.NonContrastEnhancing,
            MedianAdcTumor = model.MedianAdcTumor,
            MedianAdcCe = model.MedianAdcCe,
            MedianAdcEdema = model.MedianAdcEdema,
            MedianCbfTumor = model.MedianCbfTumor,
            MedianCbfCe = model.MedianCbfCe,
            MedianCbfEdema = model.MedianCbfEdema,
            MedianCbvTumor = model.MedianCbvTumor,
            MedianCbvCe = model.MedianCbvCe,
            MedianCbvEdema = model.MedianCbvEdema,
            MedianMttTumor = model.MedianMttTumor,
            MedianMttCe = model.MedianMttCe,
            MedianMttEdema = model.MedianMttEdema
        };
    }
}
