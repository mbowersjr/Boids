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
        private readonly List<Boid> _boids;
        private const int _numBoids = 100;

        private readonly FlockBehaviors _flockBehaviors;

        public GridRenderer Grid => MainGame.Grid;

        public Flock()
        {
            _boids = new List<Boid>(_numBoids);

            for (var i = 0; i < _numBoids; i++)
            {
                var position = new Vector2(RandomStatic.NextSingle(0f, MainGame.ScreenWidth),
                                           RandomStatic.NextSingle(0f, MainGame.ScreenHeight));
                var boid = new Boid(position, this);
                Add(boid);
            }

            _flockBehaviors = new FlockBehaviors();
        }

        public void Add(Boid boid)
        {
            _boids.Add(boid);
        }

        public void Remove(Boid boid)
        {
            _boids.Remove(boid);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Boid boid in _boids)
            {
                boid.Draw(spriteBatch);
            }
        }

        private List<Vector2> GetBorderPoints(Boid boid)
        {
            return new List<Vector2>()
            {
                new Vector2(boid.Position.X, 0),
                new Vector2(boid.Position.X, MainGame.ScreenHeight),
                new Vector2(0, boid.Position.Y),
                new Vector2(MainGame.ScreenWidth, boid.Position.Y)
            };
        }

        public void Update()
        {
            Task.Run(() =>
            {
                foreach (Boid boid in _boids)
                {
                    var avoidanceForce = _flockBehaviors.Avoidance.Avoidance(boid, _boids) * 1.5f;
                    boid.Accelerate(avoidanceForce);

                    var avoidPointsForce = _flockBehaviors.AvoidPoints.AvoidPoints(boid, GetBorderPoints(boid)) * 5f;
                    boid.Accelerate(avoidPointsForce);

                    var alignmentForce = _flockBehaviors.Alignment.Alignment(boid, _boids) / 1.5f;
                    boid.Accelerate(alignmentForce);

                    var cohesionForce = _flockBehaviors.Cohesion.Cohesion(boid, _boids) / 3f;
                    boid.Accelerate(cohesionForce);

                    boid.Accelerate(boid.Velocity / 7f);

                    boid.Run();
                }
            });
        }
    }
}
