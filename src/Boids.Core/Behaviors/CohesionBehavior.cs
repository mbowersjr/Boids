using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core;
using Boids.Core.Entities;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class CohesionBehavior : IBehavior
    {
        public string Name => "Cohesion";
        
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
        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            // Steers toward the average position of all nearby boids
            
            var force = Vector2.Zero;
            var averagePositions = Vector2.Zero;
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                var dist = Vector2.Distance(boid.Position, other.Position);

                if (dist > 0 && dist < Radius)
                {
                    averagePositions += other.Position;
                    count++;
                }
            }

            if (count > 0)
            {
                averagePositions /= count;
                force = Seek(boid, averagePositions);
            }
            
            return force;
        }

        private Vector2 Seek(Boid boid, Vector2 target)
        {
            var desired = target - boid.Position;
            desired.Normalize();
            desired *= MainGame.Options.Limits.MaxVelocity;
            
            var steering = desired - boid.Velocity;
            steering.Limit(MainGame.Options.Limits.MaxForce);

            return steering;
        }
    }
}