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

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers towards the average velocity of all nearby boids
            
            var totalForce = new Vector2();
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                // Position distance
                var distance = Vector2.Distance(boid.Position, other.Position);
                if (distance > 0f && distance < Radius)
                {
                    totalForce += other.Velocity;
                    count++;
                }
            }

            if (count > 0) 
                totalForce /= count;

            return totalForce.Length() > 0f ? totalForce : Vector2.Zero;
        }
    }
}
