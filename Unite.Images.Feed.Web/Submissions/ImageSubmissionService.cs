using Unite.Cache.Configuration.Options;
using Unite.Images.Feed.Web.Models.Base;
using Unite.Images.Feed.Web.Models.Radiomics;

namespace Unite.Images.Feed.Web.Submissions;

public class ImagesSubmissionService
{
    private readonly Repositories.MriImagesSubmissionRepository _mriImagesSubmissionRepository;
    private readonly Repositories.RadiomicsSubmissionRepository _radiomicsSubmissionRepository;

    public ImagesSubmissionService(IMongoOptions options)
    {
        _mriImagesSubmissionRepository = new Repositories.MriImagesSubmissionRepository(options);
        _radiomicsSubmissionRepository = new Repositories.RadiomicsSubmissionRepository(options);
    }

    public string AddMriImagesSubmission(Models.MriImageModel[] data)
    {
        return _mriImagesSubmissionRepository.Add(data);
    }

    public Models.MriImageModel[] FindMriImagesSubmission(string id)
    {
        return _mriImagesSubmissionRepository.Find(id)?.Document;
    }

    public void DeleteMriImagesSubmission(string id)
    {
        _mriImagesSubmissionRepository.Delete(id); 
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