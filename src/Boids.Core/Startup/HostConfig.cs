using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Boids.Core.Startup
{
    public static class HostConfig
    {
        public static void ConfigureHost(IConfigurationBuilder hostConfig)
        {
            hostConfig.SetBasePath(Directory.GetCurrentDirectory());
            hostConfig.AddJsonFile("hostsettings.json", optional: true);
            hostConfig.AddEnvironmentVariables(prefix: "BOIDS_");
            hostConfig.AddCommandLine(Environment.GetCommandLineArgs());
        }
    }
}