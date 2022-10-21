namespace Unite.Images.Feed.Web.Configuration.Options;

public class IndexingOptions
{
    /// <summary>
    /// Indexing bucket size
    /// </summary>
    public int BucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_INDEXING_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }
}
