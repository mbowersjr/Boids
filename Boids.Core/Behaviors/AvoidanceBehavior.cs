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
                var distance = direction.Length();

                if (distance > 0f && distance < Radius)
                {
                    var force = Vector2.Normalize(direction) / distance;
                    totalForce += force;
                    count++;
                }
            }

            if (count > 0)
                totalForce /= count;

            return totalForce.Length() > 0f ? totalForce : Vector2.Zero;
        }
    }
}
