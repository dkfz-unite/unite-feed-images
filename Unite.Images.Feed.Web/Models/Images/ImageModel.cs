namespace Unite.Images.Feed.Web.Models.Images;

public class ImageModel
{
    public string Id { get; set; }
    public string DonorId { get; set; }
    public DateTime? ScanningDate { get; set; }
    public int? ScanningDay { get; set; }

    public MriImageModel MriImage { get; set; }
    public CtImageModel CtImage { get; set; }

    public AnalysisModel Analysis { get; set; }


    public void Sanitise()
    {
        Id = Id?.Trim();
        DonorId = DonorId?.Trim();

        MriImage?.Sanitise();
        CtImage?.Sanitise();
        Analysis?.Sanitise();
    }
}
