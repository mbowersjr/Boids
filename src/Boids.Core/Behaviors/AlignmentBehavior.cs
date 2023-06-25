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
    public class AlignmentBehavior : IBehavior
    {
        public string Name => "Alignment";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public float? RadiusSquared => Radius == null ? 0f : Radius * Radius;
        public int? Order { get; set; }

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers towards the average velocity of all nearby boids
            
            var force = Vector2.Zero;
            var sumVelocities = Vector2.Zero;
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
                    sumVelocities += other.Velocity;
                    count++;
                }
            }

            if (count > 0)
            {
                sumVelocities /= count;
                sumVelocities.Normalize();
                sumVelocities *= MainGame.Options.Limits.MaxVelocity;

                force = sumVelocities - boid.Velocity;
                force.Limit(MainGame.Options.Limits.MaxForce);
            }

            return force;
        }
    }
}
