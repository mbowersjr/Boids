using System.Collections.Generic;
using Boids.Core.Entities;
using Microsoft.Xna.Framework;

namespace Boids.Core.Behaviors
{
    public interface IBehavior
    {
        string Name { get; }

        bool Enabled { get; set; }
        ref bool EnabledRef { get; }
        
        float Coefficient { get; set; }
        ref float CoefficientRef { get; }
        
        float Radius { get; set; }
        ref float RadiusRef { get; }
        
        float RadiusSquared { get; }
        
        int Order { get; set; }
        ref int OrderRef { get; }

        Vector2 Perform(Boid boid, IEnumerable<Boid> boids);
    }
}
