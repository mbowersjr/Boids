using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core;
using Boids.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;

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
            var distanceSquared = direction.LengthSquared();

            var totalForce = Vector2.Zero;
            
            if (distanceSquared < RadiusSquared)
            {
                totalForce = direction;
            }

            return totalForce != Vector2.Zero ? totalForce : Vector2.Zero;
        }
    }
}