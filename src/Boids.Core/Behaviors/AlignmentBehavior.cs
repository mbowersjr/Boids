using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class AlignmentBehavior : IBehavior
    {
        public string Name => "Alignment";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public float? RadiusSquared => Radius == null ? 0f : Radius * Radius;

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers towards the average velocity of all nearby boids
            
            var totalForce = Vector2.Zero;
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                var distanceSquared = Vector2.DistanceSquared(boid.Position, other.Position);
                
                if (distanceSquared < RadiusSquared)
                {
                    totalForce += other.Velocity;
                    count++;
                }
            }

            if (count > 0)
            {
                totalForce /= count;
                totalForce.Normalize();
            }

            return totalForce;
        }
    }
}
