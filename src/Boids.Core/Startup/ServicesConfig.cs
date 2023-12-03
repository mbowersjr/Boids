using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Boids.Core.Behaviors;
using Boids.Core.Configuration;
using Boids.Core.Entities;
using Boids.Core.Gui;
using Boids.Core.Gui.Console;
using Boids.Core.Gui.Views;
using Boids.Core.Services;
using Microsoft.Xna.Framework;

namespace Boids.Core.Startup
{
    public static class ServicesConfig
    {
        /// <summary>
        /// Adds services to the Dependency Injection container. 
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IEnumCacheProvider, EnumCacheProvider>();
            services.AddSingleton<InputListenerService>();
            services.AddSingleton<IFlockBehaviors, FlockBehaviors>();
            services.AddSingleton<PartitionGrid>();
            services.AddSingleton<PartitionGridRenderer>();
            services.AddSingleton<IFlock, Flock>();

            services.AddTransient<DebugConsoleState>();
            services.AddTransient<DebugConsoleLoggerProvider>();
            services.AddTransient<DebugConsoleLogger>();

            // services.AddSingleton<ImGuiRenderer>();
            services.AddSingleton<GuiManager>();
            services.AddSingleton<FlockListView>();
            services.AddSingleton<DebugConsoleView>();

            services.AddSingleton<MainGame>();
            services.AddHostedService<MainGameHostedService>();

            services.Configure<BoidsOptions>(context.Configuration.GetSection("Boids"));
        }
    }
}