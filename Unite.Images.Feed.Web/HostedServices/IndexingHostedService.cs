﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Images.Feed.Web.Configuration.Options;
using Unite.Images.Feed.Web.Handlers;

namespace Unite.Images.Feed.Web.HostedServices
{
    public class IndexingHostedService : BackgroundService
    {
        private readonly IndexingOptions _options;
        private readonly IndexingHandler _handler;
        private readonly ILogger _logger;

        public IndexingHostedService(
            IndexingOptions options,
            IndexingHandler handler,
            ILogger<IndexingHostedService> logger)
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

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _handler.Handle(_options.BucketSize);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);

                    if (exception.InnerException != null)
                    {
                        _logger.LogError(exception.InnerException.Message);
                    }
                }
                finally
                {
                    await Task.Delay(_options.Interval, cancellationToken);
                }
            }
        }
    }
}
