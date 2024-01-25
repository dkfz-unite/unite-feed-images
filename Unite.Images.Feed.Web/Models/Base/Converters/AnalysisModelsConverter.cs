namespace Unite.Images.Feed.Web.Models.Base.Converters;

public class AnalysisModelsConverter
{
    public Data.Models.AnalysisModel Convert(in AnalysisModel model)
    {
        if (model == null)
            return null;

        return new Data.Models.AnalysisModel
        {
            ReferenceId = model.Id,
            Type = model.Type,
            Date = model.Date,
            Day = model.Day,
            Parameters = model.Parameters,
            RadiomicsFeatures = model.Features?.Select(GetRadiomicsFeatureModel)
        };
    }

    public Data.Models.AnalysisModel[] Convert(in AnalysisModel[] models)
    {
        if (models == null)
            return null;

        return models.Select(model => Convert(model)).ToArray();
    }


    // private static Data.Models.ParameterModel GetParameterModel(KeyValuePair<string, string> source)
    // {
    //     return new Data.Models.ParameterModel
    //     {
    //         Name = source.Key,
    //         Value = source.Value
    //     };
    // }

    private static Data.Models.RadiomicsFeatureModel GetRadiomicsFeatureModel(KeyValuePair<string, string> source)
    {
        return new Data.Models.RadiomicsFeatureModel
        {
            Name = source.Key,
            Value = source.Value
        };
    }
}
