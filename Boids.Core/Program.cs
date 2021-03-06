using System;
using Microsoft.Extensions.Hosting;
using Boids.Core.Startup;

namespace Boids.Core
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            HostBuilderHelper.CreateHostBuilder()
                .Build()
                .Run();
        }
    }


}
