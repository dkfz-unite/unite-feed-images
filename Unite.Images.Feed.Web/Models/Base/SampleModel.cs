using System.Text.Json.Serialization;
using Unite.Data.Entities.Images.Analysis.Enums;
using Unite.Data.Entities.Images.Enums;

namespace Unite.Images.Feed.Web.Models.Base;

public class SampleModel
{
    private string _donorId;
    private string _imageId;
    private ImageType? _imageType;
    private AnalysisType? _analysisType;
    private DateOnly? _analysisDate;
    private int? _analysisDay;

    [JsonPropertyName("donor_id")]
    public virtual string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [JsonPropertyName("image_id")]
    public virtual string ImageId { get => _imageId?.Trim(); set => _imageId = value; }

    [JsonPropertyName("image_type")]
    public virtual ImageType? ImageType { get => _imageType; set => _imageType = value; }

    [JsonPropertyName("analysis_type")]
    public AnalysisType? AnalysisType { get => _analysisType; set => _analysisType = value; }

    [JsonPropertyName("analysis_date")]
    public DateOnly? AnalysisDate { get => _analysisDate; set => _analysisDate = value; }

    [JsonPropertyName("analysis_day")]
    public int? AnalysisDay { get => _analysisDay; set => _analysisDay = value; }
}
