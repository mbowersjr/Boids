using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Boids.Core.Entities
{
    public static class FlockBehaviour
    {
        public static Vector2 Avoidance(Boid boid, List<Boid> boids)
        {
            var radius = 25;
            var avoid = new Vector2();
            var count = 0;

            foreach (Boid other in boids)
            {
                if (Math.Abs((boid._cellPosition - other._cellPosition).Length()) > 1)
                {
                    continue;
                }

                var distance = Vector2.Distance(boid._position, other._position);

                if (distance < radius && distance > 0)
                {
                    avoid += (boid._position - other._position) / (float)Math.Pow(distance, 2);
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

        public static Vector2 AvoidPoints(Boid boid, List<Vector2> points)
        {
            var radius = 25;
            var avoid = new Vector2();
            var count = 0;

            foreach (Vector2 point in points)
            {
                var distance = Vector2.Distance(boid._position, point);

                if (distance < radius && distance > 0)
                {
                    avoid += (boid._position - point) / (float)Math.Pow(distance, 5);
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

        public static Vector2 Alignment(Boid boid, List<Boid> boids)
        {
            var radius = 100;
            var align = new Vector2();
            var count = 0;

            foreach (Boid other in boids)
            {
                if (Math.Abs((boid._cellPosition - other._cellPosition).Length()) > 1)
                {
                    continue;
                }

                var distance = Vector2.Distance(boid._position, other._position);

                if (distance < radius && distance > 0)
                {
                    align += other._velocity;
                    count++;
                }
            }

            if (count != 0)
            {
                align /= count;
            }

            var dir = align - boid._velocity;
            dir.Normalize();

            return dir;
        }

        public static Vector2 Cohesion(Boid boid, List<Boid> boids)
        {
            var radius = 100;
            var cohere = new Vector2();
            var count = 0;

            foreach (Boid other in boids)
            {
                if (Math.Abs((boid._cellPosition - other._cellPosition).Length()) > 1)
                {
                    continue;
                }

                var distance = Vector2.Distance(boid._position, other._position);

                if (distance < radius && distance > 0)
                {
                    cohere += other._position;
                    count++;
                }
            }

            if (count != 0)
            {
                cohere /= count;
            }

            var dir = cohere - boid._position;
            dir.Normalize();

            return dir;
        }
    }
}
