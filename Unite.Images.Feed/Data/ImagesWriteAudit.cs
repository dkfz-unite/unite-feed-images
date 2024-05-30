namespace Unite.Images.Feed.Data;

public class ImagesWriteAudit
{
    public int ImagesCreated;
    public int ImagesUpdated;

    public HashSet<int> Images = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,[
            $"{ImagesCreated} images created",
            $"{ImagesUpdated} images updated"
        ]);
    }
}
