using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core;
using Boids.Core.Entities;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class AvoidPointsBehavior : IBehavior
    {
        public string Name => "AvoidPoints";
        
        #region Radius

        private float _radius;
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public ref float RadiusRef => ref _radius;

        public float RadiusSquared => _radius == null ? 0f : _radius * _radius;

        #endregion

        #region Order

        private int _order;
        public int Order
        {
            get => _order;
            set => _order = value;
        }
        public ref int OrderRef => ref _order;

        #endregion

        #region Coefficient

        private float _coefficient;
        public float Coefficient
        {
            get => _coefficient;
            set => _coefficient = value;
        }
        public ref float CoefficientRef => ref _coefficient;

        #endregion

        #region Enabled

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }
        public ref bool EnabledRef => ref _enabled;

        #endregion
        
        private Vector2[] _avoidedPoints = new Vector2[3];

        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            var totalForce = Vector2.Zero;
            var count = 0;

            FindNearestBoundsPoints(boid, ref _avoidedPoints);
            
            foreach (var point in _avoidedPoints)
            {
                var dist = Vector2.Distance(boid.Position, point);
                if (dist > 0 && dist < Radius)
                {
                    var diff = boid.Position - point;
                    diff.Normalize();
                    diff /= dist;
                    totalForce += diff;
                    count++;
                }
            }

            if (count > 0)
            {
                totalForce /= count;
            }

            return totalForce;
        }
 
        // ReSharper disable UnusedMember.Local
        
        private static bool IsLeftHalf(Boid boid) => boid.Position.X < MainGame.ViewportAdapter.BoundingRectangle.Center.X;
        private static bool IsRightHalf(Boid boid) => !IsLeftHalf(boid);
        private static bool IsTopHalf(Boid boid) => boid.Position.Y < MainGame.ViewportAdapter.BoundingRectangle.Center.Y;
        private static bool IsBottomHalf(Boid boid) => !IsTopHalf(boid);
        
        // ReSharper restore UnusedMember.Local

        public static void FindNearestBoundsPoints(Boid boid, ref Vector2[] points)
        {
            var nearestBoundsX = IsLeftHalf(boid) ? 0f : MainGame.ViewportAdapter.ViewportWidth;
            var nearestBoundsY = IsTopHalf(boid) ? 0f : MainGame.ViewportAdapter.ViewportHeight;

            points[0].X = boid.Position.X;
            points[0].Y = nearestBoundsY;
            
            points[1].X = nearestBoundsX;
            points[1].Y = boid.Position.Y;
            
            points[2].X = nearestBoundsX;
            points[2].Y = nearestBoundsY;
        }
    }
}
