using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
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

                // Position distance
                var direction = boid.Position - other.Position;
                var distanceSquared = direction.LengthSquared();
                
                if (distanceSquared > 0f && distanceSquared < RadiusSquared)
                {
                    totalForce += other.Position + other.Velocity;
                    count++;
                }
            }

            if (count > 0) 
                totalForce /= count;

            return totalForce != Vector2.Zero ? totalForce * (Coefficient ?? 1f) : Vector2.Zero;
        }
    }
}
