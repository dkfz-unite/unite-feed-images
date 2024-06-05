using DataModels = Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Web.Models.Base.Converters;

public abstract class AnalysisModelConverter<TEntry> : ConverterBase
    where TEntry : class, new()
{
    public virtual DataModels.SampleModel Convert(AnalysisModel<TEntry> analysisModel)
    {
        var sample = GetSample(analysisModel.Sample);

        MapEntries(analysisModel, sample);

        return sample;
    }


    private DataModels.SampleModel GetSample(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Image = GetImage(sampleModel.DonorId, sampleModel.ImageId, sampleModel.ImageType),
            Analysis = GetAnalysis(sampleModel)
        };
    }

    private static DataModels.AnalysisModel GetAnalysis(SampleModel sampleModel)
    {
        return new DataModels.AnalysisModel
        {
            Type = sampleModel.AnalysisType,
            Date = sampleModel.AnalysisDate,
            Day = sampleModel.AnalysisDay
        };
    }

    protected abstract void MapEntries(AnalysisModel<TEntry> analysisModel, DataModels.SampleModel sample);
}
