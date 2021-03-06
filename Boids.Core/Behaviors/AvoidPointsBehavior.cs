using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;

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
            var totalForce = new Vector2();
            var count = 0;

            FindNearestBoundsPoints(boid, ref _avoidedPoints);
            
            foreach (var point in _avoidedPoints)
            {
                var direction = boid.Position - point;
                var distance = direction.Length();

                if (distance > 0f && distance < Radius)
                {
                    var force = Vector2.Normalize(direction) / distance;
                    totalForce += force;
                    count++;
                }
            }

            if (count > 0)
                totalForce /= count;

            return totalForce.Length() > 0f ? totalForce : Vector2.Zero;
        }
 
        private static bool IsLeftHalf(Boid boid) => boid.Position.X < MainGame.ViewportAdapter.BoundingRectangle.Center.X;
        private static bool IsRightHalf(Boid boid) => !IsLeftHalf(boid);
        private static bool IsTopHalf(Boid boid) => boid.Position.Y < MainGame.ViewportAdapter.BoundingRectangle.Center.Y;
        private static bool IsBottomHalf(Boid boid) => !IsTopHalf(boid);

        public static void FindNearestBoundsPoints(Boid boid, ref Vector2[] points)
        {
            var nearestBoundsX = IsLeftHalf(boid) ? 0f : MainGame.ViewportAdapter.BoundingRectangle.Width;
            var nearestBoundsY = IsTopHalf(boid) ? 0f : MainGame.ViewportAdapter.BoundingRectangle.Height;

            points[0].X = boid.Position.X;
            points[0].Y = nearestBoundsY;
            
            points[1].X = nearestBoundsX;
            points[1].Y = boid.Position.Y;
            
            points[2].X = nearestBoundsX;
            points[2].Y = nearestBoundsY;
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
