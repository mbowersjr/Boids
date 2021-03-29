using System;
using Microsoft.Extensions.Configuration;

namespace Boids.Core.Startup
{
    public static class AppConfig
    {
        public static void ConfigureApp(IConfigurationBuilder appConfig)
        {
            appConfig
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddCommandLine(Environment.GetCommandLineArgs());
        }
    }
}