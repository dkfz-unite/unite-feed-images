using Unite.Essentials.Tsv;

namespace Unite.Images.Feed.Web.Models.Radiomics.Binders;

public class RadiomicsTsvModelBinder : Base.Binders.AnalysisTsvModelBinder<FeatureModel>
{
    protected override ClassMap<FeatureModel> CreateMap()
    {
        return new ClassMap<FeatureModel>()
            .Map(entity => entity.Name, "name")
            .Map(entity => entity.Value, "value");
    }
}
