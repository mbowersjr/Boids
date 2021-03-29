using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class AvoidanceBehavior : IBehavior
    {
        public string Name => "Avoidance";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public float? RadiusSquared => Radius == null ? 0f : Radius * Radius;

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers away from nearby neighbors, weighting force applied by distance
            
            var totalForce = Vector2.Zero;
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                var direction = boid.Position - other.Position;
                var distanceSquared = direction.LengthSquared();

                if (distanceSquared < RadiusSquared)
                {
                    Debug.Assert(Radius != null, nameof(Radius) + " != null");
                    
                    var scale = 1f - direction.Length() / Radius.Value;
                    totalForce += Vector2.Normalize(direction) / scale;
                    count++;
                }
            }

            if (count > 1)
            {
                totalForce /= count;
            }

            return totalForce != Vector2.Zero ? totalForce : Vector2.Zero;
        }
    }
}