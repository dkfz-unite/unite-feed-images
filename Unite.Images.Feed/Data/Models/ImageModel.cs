namespace Unite.Images.Feed.Data.Models;

public abstract class ImageModel
{
    public string ReferenceId;
    public DateOnly? CreationDate;
    public int? CreationDay;

    public DonorModel Donor;
}
