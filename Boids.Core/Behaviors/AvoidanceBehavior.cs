using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
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
            
            var totalForce = new Vector2();
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                // Position distance
                var direction = boid.Position - other.Position;
                var distanceSquared = direction.LengthSquared();

                if (distanceSquared > 0f && distanceSquared < RadiusSquared)
                {
                    var force = direction / distanceSquared;
                    totalForce += force;
                    count++;
                }
            }

            if (count > 0)
                totalForce /= count;

            return totalForce.LengthSquared() > 0f ?  Vector2.Normalize(totalForce * -1f) * (Coefficient ?? 1f) : Vector2.Zero;
        }
    }
}
