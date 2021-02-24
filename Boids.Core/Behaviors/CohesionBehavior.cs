using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
using Boids.Core;

namespace Boids.Core.Behaviors
{
    // ReSharper disable once UnusedType.Global
    public class CohesionBehavior : IBehavior
    {
        public string Name => "Cohesion";
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        
        public Vector2 Perform(Boid boid, IEnumerable<Boid> boids)
        {
            var force = new Vector2();
            var count = 0;

            foreach (var other in boids)
            {
                if (other == boid)
                    continue;

                if (!other.IsActive)
                    continue;

                // Cell position distance
                if (Vector2.Distance(boid.CellPosition, other.CellPosition) > 2f)
                    continue;

                // Position distance
                var direction = other.Position - boid.Position;
                var distance = direction.Length();
                
                if (distance > 0f && distance < Radius)
                {
                    force += direction; //other.Position;
                    count++;
                }
            }

            if (count > 0)
                force /= count;

            return force.Length() > 0f ? force : Vector2.Zero;
        }
    }
}