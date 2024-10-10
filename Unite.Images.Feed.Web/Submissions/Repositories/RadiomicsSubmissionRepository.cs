using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Radiomics;

namespace Unite.Images.Feed.Web.Submissions.Repositories;

public class RadiomicsSubmissionRepository : CacheRepository<AnalysisModel<FeatureModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "img-rad";

    public RadiomicsSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
