using Unite.Images.Feed.Web.Models.Base;

namespace Unite.Images.Feed.Web.Models.Radiomics.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<FeatureModel>
{
    protected override void MapEntries(AnalysisModel<FeatureModel> source, Data.Models.SampleModel target)
    {
        target.RadFeatures = source.Entries.Distinct().Select(entry =>
        {
            return new Data.Models.Radiomics.FeatureModel
            {
                Name = entry.Name,
                Value = entry.Value
            };

        }).ToArray();
    }
}
