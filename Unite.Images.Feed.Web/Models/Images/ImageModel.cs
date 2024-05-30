using System.Text.Json.Serialization;
using Unite.Images.Feed.Web.Models.Base;

namespace Unite.Images.Feed.Web.Models;

public class ImageModel
{
    private string _id;
    private string _donorId;
    private DateOnly? _creationDate;
    private int? _creationDay;


    [JsonPropertyName("id")]
    public string Id { get => _id?.Trim(); set => _id = value; }

    [JsonPropertyName("donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [JsonPropertyName("creation_date")]
    public DateOnly? CreationDate { get => _creationDate; set => _creationDate = value; }

    [JsonPropertyName("creation_day")]
    public int? CreationDay { get => _creationDay; set => _creationDay = value; }


    [JsonPropertyName("mri_image")]
    public MriImageModel MriImage { get; set; }

    [JsonPropertyName("ct_image")]
    public CtImageModel CtImage { get; set; }
}
