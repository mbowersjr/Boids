using System;
using Boids.Core.Behaviors;
using Boids.Core.Configuration;
using Boids.Core.Entities;
using Boids.Core.Gui;
using Boids.Core.Gui.Windows;
using Boids.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoGame.Extended.Input.InputListeners;

namespace Boids.Core.Startup
{
    public static class ServicesConfig
    {
        /// <summary>
        /// Adds services to the Dependency Injection container. 
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddTransient<IEnumCacheProvider, EnumCacheProvider>();
            services.AddTransient<InputListenerService>();
            services.AddTransient<IFlock, Flock>();
            services.AddTransient<IFlockBehaviors, FlockBehaviors>();
            services.AddTransient<PartitionGrid>();
            services.AddTransient<PartitionGridRenderer>();
            
            //services.AddTransient<ConsoleWindowLoggerOptions, ConsoleWindowLoggerOptions>();
            services.AddSingleton<IDebugConsoleWindow, DebugConsoleWindow>();

            services.AddSingleton<MainGame>();
            
            services.AddHostedService<MainGameHostedService>();

            services.Configure<BoidsOptions>(context.Configuration.GetSection("Boids"));
        }
    }
}