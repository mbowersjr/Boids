using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core.Behaviors;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Boids.Core.Entities
{
    public interface IFlock
    {
        List<Boid> Boids { get; }
        IFlockBehaviors Behaviors { get; }
        bool Paused { get; set; }
        void ResetFlock();
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont);
        void Update(GameTime gameTime);
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
                InitializeBoid(boid);
                
                Boids.Add(boid);
            }
        }

        public void InitializeBoid(Boid boid)
        {
            boid.IsActive = true;
            
            var spawnArea = MainGame.ViewportAdapter.BoundingRectangle.ToRectangleF();
            spawnArea.Inflate(-200f, -200f);

            var position = MainGame.Random.NextVector2Within(ref spawnArea);
            Debug.Assert(MainGame.ViewportAdapter.BoundingRectangle.Contains(position), "Spawn position is not within viewport bounds");

            boid.Position = position;
            
            Vector2 velocity;
            MainGame.Random.NextUnitVector(out velocity);
            velocity *= MainGame.Random.NextSingle(MainGame.Options.Limits.SpawnVelocity.Range.Value);
            boid.Velocity = velocity;
            
            boid.Rotation = boid.Velocity.ToRadians();
            boid.Acceleration = Vector2.Zero;
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

        public void Update(GameTime gameTime)
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
                    
                    // if (behavior.Coefficient != null && MainGame.Options.IgnoreBehaviorCoefficients != true)
                    //     force *= behavior.Coefficient.Value;

                    boid.ApplyForce(force);
                }

                boid.Update(gameTime);
            }
        }
    }
}
