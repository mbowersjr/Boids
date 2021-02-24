using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core.Behaviors;
using MonoGame.Extended;

namespace Boids.Core.Entities
{
    public interface IFlock
    {
        List<Boid> Boids { get; }
        IFlockBehaviors Behaviors { get; }
        bool Paused { get; set; }
        void ResetFlock();
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont);
        void Update(float elapsedSeconds);
    }

    public class Flock : IFlock
    {
        public List<Boid> Boids { get; private set; } = new List<Boid>();
        public IFlockBehaviors Behaviors { get; private set; }
        public bool Paused { get; set; }

        public Flock(IFlockBehaviors behaviors)
        {
            Behaviors = behaviors;
        }

        public void ResetFlock()
        {
            Boids.Clear();
            Behaviors.Reset();

            for (var i = 0; i < MainGame.Options.Count; i++)
            {
                var boid = new Boid(this);
                Boids.Add(boid);
                boid.Reset();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            foreach (var boid in Boids)
            {
                if (!boid.IsActive)
                    continue;

                boid.Draw(gameTime, spriteBatch, spriteFont);
            }
        }

        public void Update(float elapsedSeconds)
        {
            if (Paused)
                return;
            
            foreach (var boid in Boids)
            {
                if (!boid.IsActive)
                    continue;

                foreach (var behavior in Behaviors.Behaviors)
                {
                    if (!behavior.Enabled)
                        continue;

                    var force = behavior.Perform(boid, Boids);
                    
                    if (behavior.Coefficient != null)
                        force *= behavior.Coefficient.Value;

                    boid.Acceleration += force;
                }

                boid.Acceleration.Truncate(Boid.MaxForce);

                boid.Update(elapsedSeconds);
            }
        }
    }
}
