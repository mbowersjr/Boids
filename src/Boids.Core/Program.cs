using System;
using Microsoft.Extensions.Hosting;
using Boids.Core.Startup;
using MonoGame.Extended;

namespace Boids.Core
{
    public static class Program
    {
        public static IHost Host { get; set; }

        [STAThread]
        public static void Main()
        {
            Host = HostBuilderHelper.CreateHostBuilder().Build();
            Host.Run();
        }
    }
}
