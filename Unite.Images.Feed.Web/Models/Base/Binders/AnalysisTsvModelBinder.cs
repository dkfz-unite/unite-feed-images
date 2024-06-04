using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Data.Entities.Images.Analysis.Enums;
using Unite.Data.Entities.Images.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;

namespace Unite.Images.Feed.Web.Models.Base.Binders;

public abstract class AnalysisTsvModelBinder<TModel> : IModelBinder
    where TModel : class, new()
{
    protected const string _donor_id = "donor_id";
    protected const string _image_id = "image_id";
    protected const string _image_type = "image_type";
    protected const string _analysis_type = "analysis_type";
    protected const string _analysis_date = "analysis_date";
    protected const string _analysis_day = "analysis_day";


    public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var map = CreateMap();

        var comments = new List<string>();
        
        var tsv = await reader.ReadToEndAsync();

        var model = new AnalysisModel<TModel>()
        {
            Entries = TsvReader.Read(tsv, map, comments: comments).ToArray()
        };

        ParseComments(comments, ref model);

        bindingContext.Result = ModelBindingResult.Success(model);
    }


    protected abstract ClassMap<TModel> CreateMap();


    protected virtual void ParseComments(List<string> comments, ref AnalysisModel<TModel> model)
    {
        if (comments.IsEmpty())
            return;

        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var sample = new SampleModel();

        foreach (var comment in comments)
        {
            var parts = comment.Split(':').Select(part => part.Trim()).ToArray();

            if (parts.Length < 2)
                continue;

            // Target sample
            if (parts[0].Equals(_donor_id, comparison))
                sample.DonorId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_image_id))
                sample.ImageId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_image_type, comparison))
                sample.ImageType = GetValue<ImageType?>(parts[1]);
            else if (parts[0].Equals(_analysis_type, comparison))
                sample.AnalysisType = GetValue<AnalysisType?>(parts[1]);
            else if (parts[0].Equals(_analysis_date, comparison))
                sample.AnalysisDate = GetValue<DateOnly?>(parts[1]);
            else if (parts[0].Equals(_analysis_day, comparison))
                sample.AnalysisDay = GetValue<int?>(parts[1]);
        }
        
        model.Sample = sample;
    }

    protected virtual T GetValue<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;
        else if (typeof(T) == typeof(string))
            return (T)(object)value;
        else if (typeof(T) == typeof(int?))
            return (T)(object)int.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        else if (typeof(T) == typeof(DateOnly?))
            return (T)(object)DateOnly.Parse(value, CultureInfo.InvariantCulture);
        else if (typeof(T) == typeof(AnalysisType?))
            return (T)(object)Enum.Parse<AnalysisType>(value);
        else if (typeof(T) == typeof(ImageType?))
            return (T)(object)Enum.Parse<ImageType>(value);
        else
            return (T)Convert.ChangeType(value, typeof(T));
    }
}
