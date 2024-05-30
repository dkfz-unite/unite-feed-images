using Unite.Data.Entities.Images.Enums;
using DataModels = Unite.Images.Feed.Data.Models;

namespace Unite.Images.Feed.Web.Models.Base.Converters;

public abstract class AnalysisModelConverter<TEntry> where TEntry : class, new()
{
    public virtual DataModels.SampleModel Convert(AnalysisModel<TEntry> analysisModel)
    {
        var sample = ConvertSample(analysisModel.Sample);

        MapEntries(analysisModel, sample);

        return sample;
    }


    private static DataModels.SampleModel ConvertSample(SampleModel sampleModel)
    {
        return new DataModels.SampleModel
        {
            Image = ConvertImage(sampleModel),
            Analysis = ConvertAnalysis(sampleModel)
        };
    }

    private static DataModels.ImageModel ConvertImage(SampleModel sampleModel)
    {
        if (sampleModel.ImageType == ImageType.MRI)
            return new DataModels.MriImageModel { ReferenceId = sampleModel.ImageId };
        else
            throw new NotImplementedException();
    }

    private static DataModels.AnalysisModel ConvertAnalysis(SampleModel sampleModel)
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
