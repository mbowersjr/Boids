using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    public class AlignmentBehavior
    {
        public Vector2 Alignment(Boid boid, List<Boid> boids)
        {
            var radius = 100;
            var align = new Vector2();
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
                    align += other.Velocity;
                    count++;
                }
            }

            if (count != 0)
            {
                align /= count;
            }

            var dir = align - boid.Velocity;
            dir.Normalize();

            return dir;
        }
    }
}