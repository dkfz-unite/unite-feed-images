using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Images.Feed.Web.Models.Images;

namespace Unite.Images.Feed.Web.Submissions.Repositories;

public class MrImagesSubmissionRepository : CacheRepository<MrImageModel[]>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "mr";

    public MrImagesSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
