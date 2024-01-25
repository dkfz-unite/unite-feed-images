using Unite.Data.Entities.Images.Analysis.Enums;

namespace Unite.Images.Feed.Data.Models;

public class AnalysisModel
{
    public string ReferenceId { get; set; }
    public AnalysisType? Type { get; set; }
    public DateOnly? Date { get; set; }
    public int? Day { get; set; }
    
    public Dictionary<string, string> Parameters { get; set; }
    public IEnumerable<RadiomicsFeatureModel> RadiomicsFeatures { get; set; }
}
