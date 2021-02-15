using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;
using MonoGame.Extended;

namespace Boids.Core.Behaviors
{
    public class AvoidPointsBehavior : IBehavior
    {
        public string Name => "AvoidPoints";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            var force = new Vector2();
            var count = 0;

            var points = GetNearestBoundsPoints(boid);
            foreach (var point in points)
            {
                var direction = boid.Position - point;
                var distance = direction.Length();

                if (distance > 0f && distance < Radius)
                {
                    force += direction * (1f / distance);
                    count++;
                }
            }

            if (count > 0)
                force /= count;

            return force.Length() > 0f ? force : Vector2.Zero;
        }

        public static Vector2[] GetNearestBoundsPoints(Boid boid)
        {
            var viewportBounds = MainGame.Graphics.GraphicsDevice.Viewport.Bounds;
            
            var nearestBoundsX = (boid.Position.X / 2f < viewportBounds.Center.X / 2f) ? 0f : viewportBounds.Width;
            var nearestBoundsY = (boid.Position.Y / 2f < viewportBounds.Center.Y / 2f) ? 0f : viewportBounds.Height;

            var points = new Vector2[]
            {
                new Vector2(boid.Position.X, nearestBoundsY),
                new Vector2(nearestBoundsX, boid.Position.Y),
                new Vector2(nearestBoundsX, nearestBoundsY)
            };
            
            return points;
        }
    }
}
