using Unite.Data.Entities.Images.Features.Enums;

namespace Unite.Images.Feed.Web.Models.Images;

public class AnalysisModel
{
    private string _id;
    private AnalysisType? _type;
    private DateTime? _date;
    private Dictionary<string, string> _parameters;
    private Dictionary<string, string> _features;

    public string Id { get => _id?.Trim(); set => _id = value; }
    public AnalysisType? Type { get => _type; set => _type = value; }
    public DateTime? Date { get => _date; set => _date = value; }
    public Dictionary<string, string> Parameters { get => Trim(_parameters); set => _parameters = value; }
    public Dictionary<string, string> Features { get => Trim(_features); set => _features = value; }

    private static Dictionary<string, string> Trim(Dictionary<string, string> dictionary)
    {
        return dictionary?.ToDictionary(entry => entry.Key.Trim(), entry => entry.Value.Trim());
    }
}
