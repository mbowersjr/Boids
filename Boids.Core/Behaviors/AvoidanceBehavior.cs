using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    public class AvoidanceBehavior : IBehavior
    {
        public string Name => "Avoidance";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            var force = new Vector2();
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                // Cell position distance
                if (Vector2.Distance(boid.CellPosition, other.CellPosition) > 1f)
                    continue;

                // Position distance
                var direction = boid.Position - other.Position;
                var distance = direction.Length();

                if (distance > 0f && distance < Radius)
                {
                    force += direction / (distance * distance);
                    count++;
                }
            }

            if (count > 0)
                force /= count;

            var adjustmentDirection = Vector2.Zero;
            if (force.LengthSquared() > 0f)
            {
                adjustmentDirection = Vector2.Normalize(force);
            }
            
            return adjustmentDirection;
        }
    }
}
