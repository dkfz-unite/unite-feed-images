using System.Text.Json.Serialization;

namespace Unite.Images.Feed.Web.Models.Base;

public record AnalysisModel<TEntryModel>
    where TEntryModel : class, new()
{
    private TEntryModel[] _entries;

    [JsonPropertyName("sample")]
    public SampleModel Sample { get; set; }

    [JsonPropertyName("entries")]
    public TEntryModel[] Entries { get => _entries?.Distinct().ToArray(); set => _entries = value; }
}
