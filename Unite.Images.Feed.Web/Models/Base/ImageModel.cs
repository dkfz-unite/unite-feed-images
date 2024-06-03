using System.Text.Json.Serialization;

namespace Unite.Images.Feed.Web.Models.Base;

public abstract class ImageModel
{
    protected string _id;
    protected string _donorId;
    protected DateOnly? _creationDate;
    protected int? _creationDay;


    [JsonPropertyName("id")]
    public virtual string Id { get => _id?.Trim(); set => _id = value; }

    [JsonPropertyName("donor_id")]
    public virtual string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [JsonPropertyName("creation_date")]
    public virtual DateOnly? CreationDate { get => _creationDate; set => _creationDate = value; }

    [JsonPropertyName("creation_day")]
    public virtual int? CreationDay { get => _creationDay; set => _creationDay = value; }
}
