using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Boids.Core.Configuration;

namespace Boids.Core.Services
{
    public class MainGameHostedService : IHostedService
    {
        private readonly ILogger<MainGameHostedService> _logger;
        private readonly IServiceProvider _services;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IOptionsMonitor<BoidsOptions> _options;
        private readonly IConfiguration _configuration;

        public MainGameHostedService(
            IHostApplicationLifetime appLifetime,
            IServiceProvider services,
            IOptionsMonitor<BoidsOptions> options,
            IConfiguration configuration,
            ILogger<MainGameHostedService> logger)
        {
            _appLifetime = appLifetime;
            _services = services;
            _options = options;
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                using (var game = ActivatorUtilities.CreateInstance<MainGame>(_services, cancellationToken))
                {
                    game.Run();
                }
                
                _appLifetime.StopApplication();
            });
            
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
