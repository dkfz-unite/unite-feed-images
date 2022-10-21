namespace Unite.Images.Feed.Data.Models.Audit;

public class ImagesUploadAudit
{
    public int DonorsCreated;
    public int ImagesCreated;
    public int ImagesUpdated;
    public int ImagesAnalysed;

    public HashSet<int> Images;


    public ImagesUploadAudit()
    {
        Images = new HashSet<int>();
    }


    public override string ToString()
    {
        var messages = new List<string>();

        messages.Add($"{DonorsCreated} donors created");
        messages.Add($"{ImagesCreated} images created");
        messages.Add($"{ImagesUpdated} images updated");
        messages.Add($"{ImagesAnalysed} image analysed");

        return string.Join(Environment.NewLine, messages);
    }
}
