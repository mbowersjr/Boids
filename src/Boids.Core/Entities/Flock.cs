using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core.Behaviors;
using Boids.Core.Configuration;
using Microsoft.Extensions.Options;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Boids.Core.Entities
{
    public interface IFlock
    {
        List<Boid> Boids { get; }
        IFlockBehaviors Behaviors { get; }
        void ResetFlock(RectangleF spawnArea, BoidsOptions options);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont);
        void Update(GameTime gameTime);
    }

    public class Flock : IFlock
    {
        public List<Boid> Boids { get; private set; } = new List<Boid>();
        public IFlockBehaviors Behaviors { get; private set; }
        
        private int _nextBoidId = 1;

        public Flock(IFlockBehaviors behaviors)
        {
            Behaviors = behaviors;
        }

        public void ResetFlock(RectangleF spawnArea, BoidsOptions options)
        {
            Boids.Clear();
            Behaviors.Reset();
            _nextBoidId = 1;

            for (var i = 0; i < MainGame.Options.Count; i++)
            {
                var boid = new Boid(_nextBoidId++, this);
                InitializeBoid(boid, spawnArea, options.Limits.SpawnVelocity.Range.Value);
                
                Boids.Add(boid);
            }
        }

        public void InitializeBoid(Boid boid, RectangleF spawnArea, Range<float> spawnVelocityRange)
        {
            boid.IsActive = true;
            
            // spawnArea.Inflate(-200f, -200f);

            var position = FastRandomInst.NextVector2Within(ref spawnArea);

            boid.Position = position;

            FastRandomInst.NextUnitVector(out var velocity);
            velocity *= FastRandomInst.NextSingle(spawnVelocityRange);
            boid.Velocity = velocity;
            
            boid.Rotation = boid.Velocity.ToRadians();
            boid.Acceleration = Vector2.Zero;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                              blendState: BlendState.NonPremultiplied,
                              sortMode: SpriteSortMode.Deferred,
                              transformMatrix: MainGame.ViewportAdapter.GetScaleMatrix());

            foreach (var boid in Boids)
            {
                if (!boid.IsActive)
                    continue;

                boid.Draw(gameTime, spriteBatch, spriteFont);
            }

            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var boid in Boids)
            {
                if (!boid.IsActive)
                    continue;

                foreach (var behavior in Behaviors.Behaviors)
                {
                    if (!behavior.Enabled)
                        continue;

                    var force = behavior.Perform(boid, Boids);
                    force *= behavior.Coefficient;
                    
                    boid.Acceleration += force;
                }
                
                boid.Update(gameTime, MainGame.ViewportAdapter.BoundingRectangle);
            }
        }
    }
}
