using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Boids.Core;
using Boids.Core.Entities;
using Boids.Core.Services;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once UnusedType.Global
    public class SeekMouseBehavior : IBehavior
    {
        public string Name => "SeekMouse";
        
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

        private MouseState _mouseState;
        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            _mouseState = Mouse.GetState();

            var direction = _mouseState.Position.ToVector2() - boid.Position;
            var distance = direction.Length();

            var totalForce = Vector2.Zero;
            
            if (distance < Radius)
            {
                totalForce = direction;
            }

            return totalForce;
        }

        private Vector2 Arrive(Boid boid, Vector2 target)
        {
            var desired = target - boid.Position;
            var dist = desired.Length();
            
            float mag;

            if (dist < MainGame.Options.Limits.ArrivalDistance)
            {
                mag = ScaleHelper.ScaleValue(dist, 0f, MainGame.Options.Limits.ArrivalDistance, 0f, MainGame.Options.Limits.MaxVelocity, true);
            }
            else
            {
                mag = MainGame.Options.Limits.MaxVelocity;
            }

            desired.Normalize();
            desired *= mag;
            
            var steer = desired - boid.Position;
            steer.Limit(MainGame.Options.Limits.MaxForce);
            
            return steer;
        }
    }
}