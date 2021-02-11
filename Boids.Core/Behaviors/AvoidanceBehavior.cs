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
                if (Math.Abs((boid.CellPosition - other.CellPosition).Length()) > 1)
                {
                    continue;
                }

                var distance = Vector2.Distance(boid.Position, other.Position);

                if (distance < radius && distance > 0)
                {
                    avoid += (boid.Position - other.Position) / (float)Math.Pow(distance, 2);
                    count++;
                }
            }

            if (count != 0)
            {
                avoid /= count;
            }

            if (avoid.Length() > 0)
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