// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using Boids.Core;
using MonoGame.Extended;

namespace Boids.Core.Configuration
{
    public class SpawnVelocityLimitsOptions
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public Lazy<Range<float>> Range => new Lazy<Range<float>>(() => new Range<float>(Min, Max));
    }
    
    public class LimitsOptions
    {
        public float MaxVelocity { get; set; }
        public float MaxForce { get; set; }
        public float ArrivalDistance { get; set; }
        public SpawnVelocityLimitsOptions SpawnVelocity { get; set; }
    }

    public class AvoidedPointsDisplayOptions
    {
        public bool NearestPoints { get; set; }
        public bool ActivePoints { get; set; }

        public bool Enabled => NearestPoints || ActivePoints;
    }
    
    public class BoidsOptions
    {
        public int Count { get; set; }
        public int BoidRadius { get; set; }
        public bool DisplayDistanceReferenceCircles { get; set; }
        public bool DisplayBoidPropertiesText { get; set; }
        public bool DisplayDebugData { get; set; }
        public AvoidedPointsDisplayOptions AvoidedPointsDisplay { get; set; }
        public LimitsOptions Limits { get; set; }
        public List<BehaviorOptions> Behaviors { get; set; }
        public PartitionGridOptions PartitionGrid { get; set; }
        public GraphicsOptions Graphics { get; set; }
        public ThemeOptions Theme { get; set; }
    }
}
