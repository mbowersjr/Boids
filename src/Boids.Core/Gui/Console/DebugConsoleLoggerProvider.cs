using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Boids.Core.Gui.Console
{
    public class DebugConsoleLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DebugConsoleLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string name)
        {
            var config = new DebugConsoleLoggerOptions();
            return ActivatorUtilities.CreateInstance<DebugConsoleLogger>(_serviceProvider, name, config);
        }

        public void Dispose()
        {
        }
    }
}