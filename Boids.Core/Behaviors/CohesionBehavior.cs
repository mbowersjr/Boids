using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once UnusedType.Global
    public class CohesionBehavior : IBehavior
    {
        public string Name => "Cohesion";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        
        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers toward the average position of all nearby boids
            
            var totalForce = new Vector2();
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                // Position distance
                var distance = Vector2.Distance(other.Position, boid.Position);
                
                if (distance > 0f && distance < Radius)
                {
                    totalForce += other.Position;
                    count++;
                }
            }

            if (count > 0)
                totalForce /= count;

            return totalForce.Length() > 0f ? totalForce : Vector2.Zero;
        }
    }
}