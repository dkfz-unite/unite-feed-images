namespace Unite.Images.Feed.Web.Models.Images;

public class ImageModel
{
    private string _id;
    private string _donorId;
    private DateTime? _scanningDate;
    private int? _scanningDay;

    public string Id { get => _id?.Trim(); set => _id = value; }
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }
    public DateTime? ScanningDate { get => _scanningDate; set => _scanningDate = value; }
    public int? ScanningDay { get => _scanningDay; set => _scanningDay = value; }

    public MriImageModel MriImage { get; set; }
    public CtImageModel CtImage { get; set; }
    public AnalysisModel Analysis { get; set; }

}
