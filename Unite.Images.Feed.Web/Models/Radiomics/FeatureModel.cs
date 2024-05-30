using System.Text.Json.Serialization;

namespace Unite.Images.Feed.Web.Models.Radiomics;

public record FeatureModel
{
    private string _name;
    private string _value;

    [JsonPropertyName("name")]
    public string Name { get => _name?.Trim(); set => _name = value; }

    [JsonPropertyName("value")]
    public string Value { get => _value?.Trim(); set => _value = value; }
}
