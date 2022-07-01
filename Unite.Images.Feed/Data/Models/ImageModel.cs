namespace Unite.Images.Feed.Data.Models;

public abstract class ImageModel
{
    public DateOnly? ScanningDate { get; set; }
    public int? ScanningDay { get; set; }

    public DonorModel Donor { get; set; }

    public AnalysisModel Analysis { get; set; }
}
