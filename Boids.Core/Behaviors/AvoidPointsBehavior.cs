using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

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

            var points = GetPoints(boid);
            foreach (var point in points)
            {
                var direction = boid.Position - point;
                var distance = direction.Length();

                if (distance < Radius && distance > 0f)
                {
                    force += direction / (distance * distance);
                    count++;
                }
            }

            if (count > 0)
                force /= count;


            var adjustmentDirection = Vector2.Zero;
            if (force.LengthSquared() > 0f)
            {
                adjustmentDirection = Vector2.Normalize(force);
            }
            
            return adjustmentDirection;
        }

        private static IEnumerable<Vector2> GetPoints(Boid boid)
        {
            return new Vector2[]
            {
                new (boid.Position.X, 0f),
                new (boid.Position.X, MainGame.Options.Graphics.Resolution.Y),
                new (0f, boid.Position.Y),
                new (MainGame.Options.Graphics.Resolution.X, boid.Position.Y)
            };
            
        }
    }
}
