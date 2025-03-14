using Unite.Data.Entities.Images.Analysis.Enums;

namespace Unite.Images.Feed.Data.Models;

public class AnalysisModel
{
    public AnalysisType Type;
    public DateOnly? Date;
    public int? Day;
    public Dictionary<string, string> Parameters;
}
