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
    public class CohesionBehavior : IBehavior
    {
        public string Name => "Cohesion";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public float? RadiusSquared => Radius == null ? 0f : Radius * Radius;

        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers toward the average position of all nearby boids
            
            var desiredPosition = Vector2.Zero;
            var averagePositions = Vector2.Zero;
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
                    averagePositions += other.Position;
                    count++;
                }
            }

            if (count > 0)
            {
                averagePositions /= count;
                
                desiredPosition = averagePositions - boid.Position;
                desiredPosition.Normalize();
            }

            return desiredPosition;
        }
    }
}