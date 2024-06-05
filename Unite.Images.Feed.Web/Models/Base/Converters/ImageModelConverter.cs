namespace Unite.Images.Feed.Web.Models.Base.Converters;

public abstract class ImageModelConverter<TSource, TTarget> : ConverterBase
    where TSource : ImageModel
    where TTarget : Data.Models.ImageModel, new()
{
    public TTarget Convert(in TSource source)
    {
        var target = new TTarget();

        Map(source, ref target);

        return target;
    }


    protected virtual void Map(in TSource source, ref TTarget target)
    {
        target.ReferenceId = source.Id;
        target.CreationDate = source.CreationDate;
        target.CreationDay = source.CreationDay;
        target.Donor = GetDonor(source.DonorId);
    }
}
