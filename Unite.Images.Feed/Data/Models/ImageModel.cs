namespace Unite.Images.Feed.Data.Models;

public abstract class ImageModel
{
    public string ReferenceId { get; set; }
    public DateOnly? ScanningDate { get; set; }
    public int? ScanningDay { get; set; }

    public DonorModel Donor { get; set; }
    public AnalysisModel Analysis { get; set; }
}
