using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    public class AvoidanceBehavior
    {
        public Vector2 Avoidance(Boid boid, List<Boid> boids)
        {
            var radius = 25;
            var avoid = new Vector2();
            var count = 0;

            foreach (Boid other in boids)
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

                if (distance < radius && distance > 0f)
                {
                    avoid += direction / MathF.Pow(distance, 2f);
                    count++;
                }
            }

            if (count != 0)
            {
                avoid /= count;
            }

            if (avoid.Length() > 0f)
            {
                avoid.Normalize();
            }
            else
            {
                avoid = Vector2.Zero;
            }

            return avoid;
        }
    }
}