using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Boids.Core.Services;

namespace Boids.Core
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            CreateHostBuilder(Environment.GetCommandLineArgs())
                .Build()
                .Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();
                services.AddHostedService<MainGameHostedService>();

                var boidsOptions = new BoidsOptions();
                hostContext.Configuration.Bind("Boids", boidsOptions);
                services.AddSingleton(boidsOptions);
            });

            builder.ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("hostsettings.json", optional: true);
                configHost.AddEnvironmentVariables(prefix: "BOIDS_");
                configHost.AddCommandLine(args);
            });

            builder.ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{GetAssemblyConfiguration()}.json", optional: true, reloadOnChange: true)
                    .AddCommandLine(args);
            });

            builder.ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Warning);
            });

            return builder;
        }


        private static string GetAssemblyConfiguration()
        {
#if DEBUG
            return "Debug";
#else
            return "Release";
#endif
        }
    }


}
