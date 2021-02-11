using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace Boids.Core.Services
{
    public class MainGameHostedService : IHostedService
    {
        private readonly ILogger<MainGameHostedService> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly BoidsOptions _options;

        public MainGameHostedService(BoidsOptions options, ILogger<MainGameHostedService> logger, IHostApplicationLifetime appLifetime)
        {
            _options = options;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting game loop ...");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(() =>
                {

                    using (var game = new MainGame(_options))
                    {
                        game.Run();
                    }

                    _appLifetime.StopApplication();
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exiting game ...");

            return Task.CompletedTask;
        }
    }
}