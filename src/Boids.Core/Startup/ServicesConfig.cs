using System;
using Boids.Core.Behaviors;
using Boids.Core.Configuration;
using Boids.Core.Entities;
using Boids.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddTransient<IInputListenerService, InputListenerService>();
            services.AddSingleton<MainGame>();
            services.AddTransient<IFlock, Flock>();
            services.AddTransient<IFlockBehaviors, FlockBehaviors>();
            services.AddTransient<PartitionGrid>();
            services.AddTransient<PartitionGridRenderer>();
            services.AddHostedService<MainGameHostedService>();
            
            services.Configure<BoidsOptions>(context.Configuration.GetSection("Boids"));
        }
    }
}