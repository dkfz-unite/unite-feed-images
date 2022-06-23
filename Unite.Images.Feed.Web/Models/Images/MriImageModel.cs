namespace Unite.Images.Feed.Web.Models.Images;

public class MriImageModel
{
    public double? WholeTumor { get; set; }
    public double? ContrastEnhancing { get; set; }
    public double? NonContrastEnhancing { get; set; }
    public double? MedianAdcTumor { get; set; }
    public double? MedianAdcCe { get; set; }
    public double? MedianAdcEdema { get; set; }
    public double? MedianCbfTumor { get; set; }
    public double? MedianCbfCe { get; set; }
    public double? MedianCbfEdema { get; set; }
    public double? MedianCbvTumor { get; set; }
    public double? MedianCbvCe { get; set; }
    public double? MedianCbvEdema { get; set; }
    public double? MedianMttTumor { get; set; }
    public double? MedianMttCe { get; set; }
    public double? MedianMttEdema { get; set; }


    public void Sanitise()
    {

    }
}
