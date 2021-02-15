using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Boids.Core;
using Boids.Core.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Boids.Core.Behaviors
{
    public interface IFlockBehaviors
    {
        List<IBehavior> Behaviors { get; }
        void LoadBehaviors();
        void Reset();
        IBehavior GetBehavior(string name);
    }

    public class FlockBehaviors : IFlockBehaviors
    {
        public List<IBehavior> Behaviors { get; private set; } = new List<IBehavior>();

        private readonly ILogger<FlockBehaviors> _logger;

        public FlockBehaviors(ILogger<FlockBehaviors> logger)
        {
            _logger = logger;
        }

        public void Reset()
        {
            Behaviors.Clear();
            LoadBehaviors();
        }

        public IBehavior GetBehavior(string name)
        {
            return Behaviors.FirstOrDefault(b => b.Name.EqualsIgnoreCase(name));
        }
        
        public void LoadBehaviors()
        {
            _logger.LogInformation("Loading behaviors ...");

            var assembly = typeof(IBehavior).Assembly;
            _logger.LogDebug("Using assembly {Assembly} ({AssemblyLocation})", assembly.FullName, assembly.Location);

            var interfaceType = typeof(IBehavior);
            var behaviorTypes =
                assembly.GetTypes()
                    .Where(t => t.IsClass && interfaceType.IsAssignableFrom(t))
                    .ToList();

            var typeNames = string.Join(", ", behaviorTypes.Select(bt => bt.Name));
            _logger.LogDebug("Found {Count} behavior types in assembly: {TypeNames}", behaviorTypes.Count, typeNames);

            foreach (var type in behaviorTypes)
            {
                var instance = (IBehavior)Activator.CreateInstance(type);

                if (instance == null)
                {
                    _logger.LogWarning("Activator.CreateInstance returned a null instance of the behavior");
                    continue;
                }

                _logger.LogDebug("Loaded behavior '{BehaviorName}'", instance.Name);

                var options = MainGame.Options.Behaviors.FirstOrDefault(b => b.Name.EqualsIgnoreCase(instance.Name));
                if (options != null)
                {
                    _logger.LogDebug("Applying settings to '{BehaviorName}' behavior", instance.Name);

                    instance.Enabled = options.Enabled;
                    instance.Coefficient = options.Coefficient;
                    instance.Radius = options.Radius;
                }
                else
                {
                    _logger.LogDebug("No settings found for '{BehaviorName}' behavior", instance.Name);

                    instance.Enabled = true;
                    instance.Coefficient = 1f;
                    instance.Radius = 25f;
                }

                Behaviors.Add(instance);
            }
        }
    }
}
