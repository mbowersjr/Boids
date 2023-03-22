using System.Collections.Generic;
using Boids.Core.Entities;
using Microsoft.Xna.Framework;

namespace Boids.Core.Behaviors
{
    public interface IBehavior
    {
        string Name { get; }
        bool Enabled { get; set; }
        float? Coefficient { get; set; }
        float? Radius { get; set; }
        float? RadiusSquared { get; }
        int? Order { get; set; }

        Vector2 Perform(Boid boid, IEnumerable<Boid> boids);
    }
}
