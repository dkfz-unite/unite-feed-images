using Unite.Essentials.Extensions;
using Unite.Images.Feed.Web.Handlers.Submission;

namespace Unite.Images.Feed.Web.Workers;

public class SubmissionsWorker : BackgroundService
{
    private readonly MriImagesSubmissionHandler _mriImagesSubmissionHandler;
    private readonly RadiomicsSubmissionHandler _radiomicsSubmissionHandler;
    private readonly ILogger _logger;

    public SubmissionsWorker(MriImagesSubmissionHandler mriImagesSubmissionHandler,
            RadiomicsSubmissionHandler radiomicsSubmissionHandler,
            ILogger<SubmissionsWorker> logger)
    {
            _mriImagesSubmissionHandler = mriImagesSubmissionHandler;
            _radiomicsSubmissionHandler = radiomicsSubmissionHandler;
            _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Submissions worker started");

        stoppingToken.Register(() => _logger.LogInformation("Submissions worker stopped"));

            // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _mriImagesSubmissionHandler.Handle();
                _radiomicsSubmissionHandler.Handle();
            }
            catch (Exception exception)
            {
                _logger.LogError("{error}", exception.GetShortMessage());
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}