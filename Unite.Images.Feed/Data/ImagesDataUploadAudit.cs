namespace Unite.Images.Feed.Data;

public class ImagesDataUploadAudit
{
    public int DonorsCreated;
    public int ImagesCreated;
    public int ImagesUpdated;
    public int ImagesAnalysed;

    public HashSet<int> Images = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,[
            $"{DonorsCreated} donors created",
            $"{ImagesCreated} images created",
            $"{ImagesUpdated} images updated",
            $"{ImagesAnalysed} images analysed"
        ]);
    }
}
