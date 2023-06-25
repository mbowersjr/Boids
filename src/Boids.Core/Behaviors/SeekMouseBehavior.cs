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
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public float? RadiusSquared => Radius == null ? 0f : Radius * Radius;
        public int? Order { get; set; }

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