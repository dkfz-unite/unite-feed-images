using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Essentials.Tsv;

namespace Unite.Images.Feed.Web.Models.Binders;

public class MriImageTsvModelsBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var tsv = await reader.ReadToEndAsync();

        var map = new ClassMap<MriImageModel>()
            .Map(entity => entity.Id, "id")
            .Map(entity => entity.DonorId, "donor_id")
            .Map(entity => entity.CreationDate, "creation_date")
            .Map(entity => entity.CreationDay, "creation_day")
            .Map(entity => entity.WholeTumor, "whole_tumor")
            .Map(entity => entity.ContrastEnhancing, "contrast_enhancing")
            .Map(entity => entity.NonContrastEnhancing, "non_contrast_enhancing")
            .Map(entity => entity.MedianAdcTumor, "median_adc_tumor")
            .Map(entity => entity.MedianAdcCe, "median_adc_ce")
            .Map(entity => entity.MedianAdcEdema, "median_adc_edema")
            .Map(entity => entity.MedianCbfTumor, "median_cbf_tumor")
            .Map(entity => entity.MedianCbfCe, "median_cbf_ce")
            .Map(entity => entity.MedianCbfEdema, "median_cbf_edema")
            .Map(entity => entity.MedianCbvTumor, "median_cbv_tumor")
            .Map(entity => entity.MedianCbvCe, "median_cbv_ce")
            .Map(entity => entity.MedianCbvEdema, "median_cbv_edema")
            .Map(entity => entity.MedianMttTumor, "median_mtt_tumor")
            .Map(entity => entity.MedianMttCe, "median_mtt_ce")
            .Map(entity => entity.MedianMttEdema, "median_mtt_edema");

        var models = TsvReader.Read(tsv, map).ToArray();

        bindingContext.Result = ModelBindingResult.Success(models);
    }
}
