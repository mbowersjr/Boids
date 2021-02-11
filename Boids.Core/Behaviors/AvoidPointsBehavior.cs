using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    public class AvoidPointsBehavior
    {
        public Vector2 AvoidPoints(Boid boid, List<Vector2> points)
        {
            var radius = 25;
            var avoid = new Vector2();
            var count = 0;

            foreach (Vector2 point in points)
            {
                var direction = boid.Position - point;
                var distance = direction.Length();

                if (distance < radius && distance > 0)
                {
                    avoid += direction / MathF.Pow(distance, 5f);
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