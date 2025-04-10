using Unite.Images.Feed.Web.Models.Base.Converters;

namespace Unite.Images.Feed.Web.Models.Images.Converters;

public class MrImageModelConverter : ImageModelConverter<MrImageModel, Data.Models.MrImageModel>
{
    protected override void Map(in MrImageModel source, ref Data.Models.MrImageModel target)
    {
        base.Map(source, ref target);

        target.WholeTumor = source.WholeTumor;
        target.ContrastEnhancing = source.ContrastEnhancing;
        target.NonContrastEnhancing = source.NonContrastEnhancing;
        target.MedianAdcTumor = source.MedianAdcTumor;
        target.MedianAdcCe = source.MedianAdcCe;
        target.MedianAdcEdema = source.MedianAdcEdema;
        target.MedianCbfTumor = source.MedianCbfTumor;
        target.MedianCbfCe = source.MedianCbfCe;
        target.MedianCbfEdema = source.MedianCbfEdema;
        target.MedianCbvTumor = source.MedianCbvTumor;
        target.MedianCbvCe = source.MedianCbvCe;
        target.MedianCbvEdema = source.MedianCbvEdema;
        target.MedianMttTumor = source.MedianMttTumor;
        target.MedianMttCe = source.MedianMttCe;
        target.MedianMttEdema = source.MedianMttEdema;
    }
}
