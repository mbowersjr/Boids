using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core;
using Boids.Core.Entities;

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
        public int? Order { get; set; }

        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers toward the average position of all nearby boids
            
            var force = Vector2.Zero;
            var averagePositions = Vector2.Zero;
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
                    averagePositions += other.Position;
                    count++;
                }
            }

            if (count > 0)
            {
                averagePositions /= count;
                force = Seek(boid, averagePositions);
            }
            
            return force;
        }

        private Vector2 Seek(Boid boid, Vector2 target)
        {
            var desired = target - boid.Position;
            desired.Normalize();
            desired *= MainGame.Options.Limits.MaxVelocity;
            
            var steering = desired - boid.Velocity;
            steering.Limit(MainGame.Options.Limits.MaxForce);

            return steering;
        }
    }
}