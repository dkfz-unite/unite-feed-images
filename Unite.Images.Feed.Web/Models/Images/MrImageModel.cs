using System.Text.Json.Serialization;
using Unite.Images.Feed.Web.Models.Base;

namespace Unite.Images.Feed.Web.Models.Images;

public class MrImageModel : ImageModel
{
    [JsonPropertyName("whole_tumor")]
    public double? WholeTumor { get; set; }

    [JsonPropertyName("contrast_enhancing")]
    public double? ContrastEnhancing { get; set; }

    [JsonPropertyName("non_contrast_enhancing")]
    public double? NonContrastEnhancing { get; set; }

    [JsonPropertyName("median_adc_tumor")]
    public double? MedianAdcTumor { get; set; }

    [JsonPropertyName("median_adc_ce")]
    public double? MedianAdcCe { get; set; }

    [JsonPropertyName("median_adc_edema")]
    public double? MedianAdcEdema { get; set; }

    [JsonPropertyName("median_cbf_tumor")]
    public double? MedianCbfTumor { get; set; }

    [JsonPropertyName("median_cbf_ce")]
    public double? MedianCbfCe { get; set; }

    [JsonPropertyName("median_cbf_edema")]
    public double? MedianCbfEdema { get; set; }

    [JsonPropertyName("median_cbv_tumor")]
    public double? MedianCbvTumor { get; set; }

    [JsonPropertyName("median_cbv_ce")]
    public double? MedianCbvCe { get; set; }

    [JsonPropertyName("median_cbv_edema")]
    public double? MedianCbvEdema { get; set; }

    [JsonPropertyName("median_mtt_tumor")]
    public double? MedianMttTumor { get; set; }

    [JsonPropertyName("median_mtt_ce")]
    public double? MedianMttCe { get; set; }
    
    [JsonPropertyName("median_mtt_edema")]
    public double? MedianMttEdema { get; set; }
}
