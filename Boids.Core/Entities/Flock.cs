using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core.Behaviors;

namespace Boids.Core.Entities
{
    public class Flock
    {
        private List<Boid> _boids;
        private readonly FlockBehaviors _flockBehaviors;

        public PartitionGrid _grid;

        public Flock()
        {
            ResetFlock();

            _flockBehaviors = new FlockBehaviors();
        }

        public void ResetFlock()
        {
            _boids = new List<Boid>(MainGame.Options.Count);

            for (var i = 0; i < MainGame.Options.Count; i++)
            {
                var boid = new Boid(this);
                _boids.Add(boid);
                ResetBoid(boid);
            }
        }

        public void ResetBoid(Boid boid)
        {
            boid.IsActive = true;
            boid.Position = new Vector2(RandomStatic.NextSingle(0f, MainGame.Options.Graphics.Resolution.X),
                                        RandomStatic.NextSingle(0f, MainGame.Options.Graphics.Resolution.Y));
            boid.CellPosition = MainGame.Grid.GetCellPosition(boid.Position);
            boid.Velocity = RandomStatic.NextUnitVector() * RandomStatic.NextSingle(1f, 2f);
            boid.Acceleration = Vector2.Zero;
        }

        public void Add(Boid boid)
        {
            boid.IsActive = true;
        }

        public void Remove(Boid boid)
        {
            boid.IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Boid boid in _boids)
            {
                if (!boid.IsActive)
                    continue;

                boid.Draw(spriteBatch);
            }
        }

        private List<Vector2> GetBorderPoints(Boid boid)
        {
            return new List<Vector2>()
            {
                new Vector2(boid.Position.X, 0),
                new Vector2(boid.Position.X, MainGame.Options.Graphics.Resolution.Y),
                new Vector2(0, boid.Position.Y),
                new Vector2(MainGame.Options.Graphics.Resolution.X, boid.Position.Y)
            };
        }

        public void Update()
        {
            foreach (Boid boid in _boids)
            {
                if (!boid.IsActive)
                    continue;

                if (MainGame.Options.Behaviors.Avoidance)
                {
                    var avoidanceForce = _flockBehaviors.Avoidance.Avoidance(boid, _boids) * 1.5f;
                    boid.Accelerate(avoidanceForce);
                }

                if (MainGame.Options.Behaviors.AvoidPoints)
                {
                    var avoidPointsForce = _flockBehaviors.AvoidPoints.AvoidPoints(boid, GetBorderPoints(boid)) * 5f;
                    boid.Accelerate(avoidPointsForce);
                }

                if (MainGame.Options.Behaviors.Alignment)
                {
                    var alignmentForce = _flockBehaviors.Alignment.Alignment(boid, _boids) / 1.5f;
                    boid.Accelerate(alignmentForce);
                }

                if (MainGame.Options.Behaviors.Cohesion)
                {
                    var cohesionForce = _flockBehaviors.Cohesion.Cohesion(boid, _boids) / 3f;
                    boid.Accelerate(cohesionForce);
                }

                boid.Accelerate(boid.Velocity / 7f);

                boid.Run();
            }
        }
    }
}
