using Unite.Essentials.Extensions;
using Unite.Images.Feed.Web.Configuration.Options;
using Unite.Images.Feed.Web.Handlers;

namespace Unite.Images.Feed.Web.Workers;

public class IndexingWorker : BackgroundService
{
    private readonly ImagesIndexingOptions _options;
    private readonly ImagesIndexingHandler _handler;
    private readonly ILogger _logger;

    public IndexingWorker(
        ImagesIndexingOptions options,
        ImagesIndexingHandler handler,
        ILogger<IndexingWorker> logger)
    {
        _options = options;
        _handler = handler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Indexing service started");

        cancellationToken.Register(() => _logger.LogInformation("Indexing service stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, cancellationToken);

        try
        {
            await _handler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _handler.Handle(_options.BucketSize);
            }
            catch (Exception exception)
            {
                _logger.LogError("{error}", exception.GetShortMessage());
            }
            finally
            {
                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
