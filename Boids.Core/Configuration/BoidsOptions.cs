using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using Boids.Core;

namespace Boids.Core.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BoidsOptions
    {
        public int Count { get; set; }
        public bool DisplayAvoidedPointLines { get; set; }
        public bool DisplayBoidProperties { get; set; }
        
        public List<BehaviorOptions> Behaviors { get; set; }
        public PartitionGridOptions PartitionGrid { get; set; }
        public GraphicsOptions Graphics { get; set; }
        public ThemeOptions Theme { get; set; }
    }
}
