using Boids.Core.Gui;
using Boids.Core.Gui.Console;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Boids.Core.Startup
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);

            logging.AddSimpleConsole(configure =>
            {
                configure.SingleLine = true;
                configure.IncludeScopes = true;
            });
        }
    }
}