using Unite.Images.Feed.Web.Models.Base.Converters;

namespace Unite.Images.Feed.Web.Models.Converters;

public class MriImageModelConverter : ImageModelConverter<MriImageModel, Data.Models.MriImageModel>
{
    protected override void Map(in MriImageModel source, ref Data.Models.MriImageModel target)
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
