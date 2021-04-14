using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Boids.Core.Services
{
    public class MainGameHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IServiceProvider _services;
        private readonly ILogger<MainGameHostedService> _logger;
        private MainGame _game;

        public MainGameHostedService(IHostApplicationLifetime appLifetime,
                                     IServiceProvider services,
                                     ILogger<MainGameHostedService> logger)
        {
            _appLifetime = appLifetime;
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogTrace("Application lifetime started");
                
                _logger.LogTrace("Getting MainGame instance ...");
                _game = _services.GetRequiredService<MainGame>();

                _logger.LogTrace("Beginning game loop ...");
                
                _game.Run();
                
                _logger.LogTrace("Game loop stopped");
                
                _appLifetime.StopApplication();
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Application lifetime stopping ...");
            
            _logger.LogTrace("Exiting game ...");
            _game.Exit();
            
            _logger.LogTrace("Application lifetime stopped");

            return Task.CompletedTask;
        }
    }
}
