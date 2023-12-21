using System.Text.Json.Serialization;

namespace Unite.Images.Feed.Web.Models.Images;

public class ImageModel
{
    private string _id;
    private string _donorId;
    private DateOnly? _scanningDate;
    private int? _scanningDay;


    [JsonPropertyName("id")]
    public string Id { get => _id?.Trim(); set => _id = value; }

    [JsonPropertyName("donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [JsonPropertyName("scanning_date")]
    public DateOnly? ScanningDate { get => _scanningDate; set => _scanningDate = value; }

    [JsonPropertyName("scanning_day")]
    public int? ScanningDay { get => _scanningDay; set => _scanningDay = value; }


    [JsonPropertyName("mri_image")]
    public MriImageModel MriImage { get; set; }

    [JsonPropertyName("ct_image")]
    public CtImageModel CtImage { get; set; }

    [JsonPropertyName("analysis")]
    public AnalysisModel Analysis { get; set; }
}
