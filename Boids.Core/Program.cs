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
using Boids.Core.Behaviors;
using Boids.Core.Entities;
using Boids.Core.Configuration;

namespace Boids.Core
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            CreateHostBuilder(Environment.GetCommandLineArgs())
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args).UseConsoleLifetime();

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<IEnumCacheProvider, EnumCacheProvider>();
                services.AddTransient<IInputListenerService, InputListenerService>();
                services.AddSingleton<MainGame>();
                services.AddTransient<IFlock, Flock>();
                services.AddTransient<IFlockBehaviors, FlockBehaviors>();
                services.AddTransient<PartitionGrid>();
                services.AddTransient<PartitionGridRenderer>();
                services.AddHostedService<MainGameHostedService>();
                
                services.Configure<BoidsOptions>(hostContext.Configuration.GetSection("Boids"));
            });

            builder.ConfigureHostConfiguration(hostConfig =>
            {
                hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                hostConfig.AddJsonFile("hostsettings.json", optional: true);
                hostConfig.AddEnvironmentVariables(prefix: "BOIDS_");
                hostConfig.AddCommandLine(args);
            });

            builder.ConfigureAppConfiguration(appConfig =>
            {
                appConfig
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{GetAssemblyConfiguration()}.json", optional: true, reloadOnChange: true)
                    .AddCommandLine(args);
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Warning);

                logging.AddSimpleConsole(configure =>
                {
                    configure.SingleLine = true;
                    configure.IncludeScopes = true;
                });
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
