using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;
using MonoGame.Extended;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AvoidPointsBehavior : IBehavior
    {
        public string Name => "AvoidPoints";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }

        private Vector2[] _avoidedPoints = new Vector2[3];

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            var force = new Vector2();
            var count = 0;

            UpdateNearestAvoidedPoints(boid, ref _avoidedPoints);
            
            foreach (var point in _avoidedPoints)
            {
                var direction = boid.Position - point;
                var distance = direction.Length();

                if (distance > 0f && distance < Radius)
                {
                    force += direction / (1f / distance);
                    count++;
                }
            }

            // if (count > 0)
            //     force /= count;

            return force.Length() > 0f ? force : Vector2.Zero;
        }

        public static void UpdateNearestAvoidedPoints(Boid boid, ref Vector2[] points)
        {
            var viewportBounds = MainGame.ViewportAdapter.BoundingRectangle;
            
            var nearestBoundsX = (boid.Position.X / 2f < viewportBounds.Center.X / 2f) ? 0f : viewportBounds.Width;
            var nearestBoundsY = (boid.Position.Y / 2f < viewportBounds.Center.Y / 2f) ? 0f : viewportBounds.Height;

            points[0].X = boid.Position.X;
            points[0].Y = nearestBoundsY;
            points[1].X = nearestBoundsX;
            points[1].Y = boid.Position.Y;
            points[2].X = nearestBoundsX;
            points[2].Y = nearestBoundsY;
        }
    }
}
