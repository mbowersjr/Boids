using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core;
using Boids.Core.Entities;

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
        public int? Order { get; set; }

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

                var dist = Vector2.Distance(boid.Position, other.Position);
                if (dist > 0 && dist < Radius)
                {
                    var diff = boid.Position - other.Position;
                    diff.Normalize();
                    diff /= dist;
                    totalForce += diff;
                    count++;
                }
            }

            if (count > 0)
            {
                totalForce /= count;
            }

            if (totalForce.Length() > 0)
            {
                totalForce.Normalize();
                totalForce *= MainGame.Options.Limits.MaxVelocity;
                totalForce -= boid.Velocity;
                totalForce.Limit(MainGame.Options.Limits.MaxForce);
            }

            return totalForce;
        }
    }
}
