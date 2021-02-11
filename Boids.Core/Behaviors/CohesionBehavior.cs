using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    public class CohesionBehavior
    {
        public Vector2 Cohesion(Boid boid, List<Boid> boids)
        {
            var radius = 100;
            var cohere = new Vector2();
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
                var distance = Vector2.Distance(boid.Position, other.Position);
                if (distance < radius && distance > 0f)
                {
                    cohere += other.Position;
                    count++;
                }
            }

            if (count != 0)
            {
                cohere /= count;
            }

            return Vector2.Normalize(cohere - boid.Position);
        }
    }
}