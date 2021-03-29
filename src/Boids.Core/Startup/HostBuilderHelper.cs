using System;
using Microsoft.Extensions.Hosting;

namespace Boids.Core.Startup
{
    public static class HostBuilderHelper
    {
        public static IHostBuilder CreateHostBuilder()
        {
            var builder = 
                Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                    .UseConsoleLifetime();

            builder.ConfigureServices(ServicesConfig.ConfigureServices);
            builder.ConfigureHostConfiguration(HostConfig.ConfigureHost);
            builder.ConfigureAppConfiguration(AppConfig.ConfigureApp);
            builder.ConfigureLogging(LoggingConfig.ConfigureLogging);

            return builder;
        }
    }
}