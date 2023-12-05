using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Boids.Core;
using Boids.Core.Entities;

namespace Boids.Core.Behaviors
{
    public interface IFlockBehaviors
    {
        List<IBehavior> Behaviors { get; }
        void LoadBehaviors();
        void Reset();

        IBehavior GetBehavior(string name);
        IEnumerable<IBehavior> EnumerateBehaviors();
    }

    public class FlockBehaviors : IFlockBehaviors
    {
        public List<IBehavior> Behaviors { get; private set; } = new List<IBehavior>();
        
        private List<IBehavior> _orderedBehaviors;

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
        
        public IEnumerable<IBehavior> EnumerateBehaviors()
        {
            return _orderedBehaviors;
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
                    instance.Coefficient = options.Coefficient.GetValueOrDefault(1f);
                    instance.Radius = options.Radius.GetValueOrDefault(10f);
                    instance.Order = options.Order.GetValueOrDefault();
                }
                else
                {
                    _logger.LogDebug("No settings found for '{BehaviorName}' behavior", instance.Name);

                    instance.Enabled = true;
                    instance.Coefficient = 1f;
                    instance.Radius = 25f;
                    instance.Order = 0;
                }

                Behaviors.Add(instance);
            }

            _orderedBehaviors = Behaviors.OrderBy(x => x.Order != 0).ThenBy(x => x.Order).ThenBy(x => x.Name).ToList();

            _logger.LogDebug("Behavior excecution order:");
            for (int i = 0; i < _orderedBehaviors.Count; i++)
            {
                var behavior = _orderedBehaviors[i];
                if (behavior.Order != 0)
                {
                    _logger.LogDebug("#{Index}. {BehaviorName} (Order: {BehaviorOrder})", i+1, behavior.Name, behavior.Order);
                }
                else
                {
                    _logger.LogDebug("#{Index}. {BehaviorName}", i+1, behavior.Name);
                }
                

            }
        }
    }
}
