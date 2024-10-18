using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Images.Feed.Web.Models;

namespace Unite.Images.Feed.Web.Submissions.Repositories;

public class MriImagesSubmissionRepository : CacheRepository<MriImageModel[]>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "mri";

    public MriImagesSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
