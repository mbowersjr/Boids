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
    // ReSharper disable once UnusedType.Global
    public class SeekMouseBehavior : IBehavior
    {
        public string Name => "SeekMouse";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public float? RadiusSquared => Radius == null ? 0f : Radius * Radius;

        private MouseState _mouseState;
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            _mouseState = Mouse.GetState();

            var direction = _mouseState.Position.ToVector2() - boid.Position;
            var distanceSquared = direction.LengthSquared();

            Vector2 force = Vector2.Zero;
            if (distanceSquared > 0f && distanceSquared < RadiusSquared)
            {
                force = direction;
            }

            return force != Vector2.Zero ? Vector2.Normalize(force) * (Coefficient ?? 1f) : Vector2.Zero;
        }
    }
}