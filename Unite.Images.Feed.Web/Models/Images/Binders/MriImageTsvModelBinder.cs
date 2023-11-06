using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Essentials.Tsv;

namespace Unite.Images.Feed.Web.Models.Images.Binders;

public class MriImageTsvModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var tsv = await reader.ReadToEndAsync();

        var map = new ClassMap<ImageModel>()
            .Map(entity => entity.Id, "id")
            .Map(entity => entity.DonorId, "donor_id")
            .Map(entity => entity.ScanningDate, "scanning_date")
            .Map(entity => entity.ScanningDay, "scanning_day")
            .Map(entity => entity.MriImage.WholeTumor, "whole_tumor")
            .Map(entity => entity.MriImage.ContrastEnhancing, "contrast_enhancing")
            .Map(entity => entity.MriImage.NonContrastEnhancing, "non_contrast_enhancing")
            .Map(entity => entity.MriImage.MedianAdcTumor, "median_adc_tumor")
            .Map(entity => entity.MriImage.MedianAdcCe, "median_adc_ce")
            .Map(entity => entity.MriImage.MedianAdcEdema, "median_adc_edema")
            .Map(entity => entity.MriImage.MedianCbfTumor, "median_cbf_tumor")
            .Map(entity => entity.MriImage.MedianCbfCe, "median_cbf_ce")
            .Map(entity => entity.MriImage.MedianCbfEdema, "median_cbf_edema")
            .Map(entity => entity.MriImage.MedianCbvTumor, "median_cbv_tumor")
            .Map(entity => entity.MriImage.MedianCbvCe, "median_cbv_ce")
            .Map(entity => entity.MriImage.MedianCbvEdema, "median_cbv_edema")
            .Map(entity => entity.MriImage.MedianMttTumor, "median_mtt_tumor")
            .Map(entity => entity.MriImage.MedianMttCe, "median_mtt_ce")
            .Map(entity => entity.MriImage.MedianMttEdema, "median_mtt_edema");

        var model = TsvReader.Read(tsv, map).ToArray();

        bindingContext.Result = ModelBindingResult.Success(model);
    }
}

