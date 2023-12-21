using System.Text.Json.Serialization;
using Unite.Data.Entities.Images.Analysis.Enums;

namespace Unite.Images.Feed.Web.Models.Images;

public class AnalysisModel
{
    private string _id;
    private AnalysisType? _type;
    private DateOnly? _date;
    private int? _day;
    private Dictionary<string, string> _parameters;
    private Dictionary<string, string> _features;

    [JsonPropertyName("id")]
    public string Id { get => _id?.Trim(); set => _id = value; }

    [JsonPropertyName("type")]
    public AnalysisType? Type { get => _type; set => _type = value; }

    [JsonPropertyName("date")]
    public DateOnly? Date { get => _date; set => _date = value; }

    [JsonPropertyName("day")]
    public int? Day { get => _day; set => _day = value; }

    [JsonPropertyName("parameters")]
    public Dictionary<string, string> Parameters { get => Trim(_parameters); set => _parameters = value; }

    [JsonPropertyName("features")]
    public Dictionary<string, string> Features { get => Trim(_features); set => _features = value; }


    private static Dictionary<string, string> Trim(Dictionary<string, string> dictionary)
    {
        return dictionary?.ToDictionary(entry => entry.Key.Trim(), entry => entry.Value.Trim());
    }
}
