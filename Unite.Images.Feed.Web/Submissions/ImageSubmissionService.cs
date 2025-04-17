using Unite.Cache.Configuration.Options;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Images;
using Unite.Images.Feed.Web.Models.Radiomics;

namespace Unite.Images.Feed.Web.Submissions;

public class ImagesSubmissionService
{
    private readonly Repositories.MrImagesSubmissionRepository _mrImagesSubmissionRepository;
    private readonly Repositories.RadiomicsSubmissionRepository _radiomicsSubmissionRepository;

    public ImagesSubmissionService(IMongoOptions options)
    {
        _mrImagesSubmissionRepository = new Repositories.MrImagesSubmissionRepository(options);
        _radiomicsSubmissionRepository = new Repositories.RadiomicsSubmissionRepository(options);
    }

    public string AddMrImagesSubmission(MrImageModel[] data)
    {
        return _mrImagesSubmissionRepository.Add(data);
    }

    public MrImageModel[] FindMrImagesSubmission(string id)
    {
        return _mrImagesSubmissionRepository.Find(id)?.Document;
    }

    public void DeleteMrImagesSubmission(string id)
    {
        _mrImagesSubmissionRepository.Delete(id); 
    }

    public string AddRadiomicsSubmission(AnalysisModel<FeatureModel> data)
    {
        return _radiomicsSubmissionRepository.Add(data);
    }

    public AnalysisModel<FeatureModel> FindRadiomicsSubmission(string id)
    {
        return _radiomicsSubmissionRepository.Find(id)?.Document;
    }

    public void DeleteRadiomicsSubmission(string id)
    {
        _radiomicsSubmissionRepository.Delete(id);
    }
}