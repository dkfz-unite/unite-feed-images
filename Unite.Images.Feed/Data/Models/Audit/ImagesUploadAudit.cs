namespace Unite.Images.Feed.Data.Models.Audit;

public class ImagesUploadAudit
{
    public int DonorsCreated;
    public int ImagesCreated;
    public int ImagesUpdated;
    public int ImagesAnalysed;

    public HashSet<int> Images = [];

    public override string ToString()
    {
        var messages = new List<string>
        {
            $"{DonorsCreated} donors created",
            $"{ImagesCreated} images created",
            $"{ImagesUpdated} images updated",
            $"{ImagesAnalysed} image analysed"
        };

        return string.Join(Environment.NewLine, messages);
    }
}
