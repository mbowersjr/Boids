using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core.Entities
{
    public class Flock
    {
        private static Random _rand = new Random();

        private List<Boid> _boids;
        private int _numBoids = 100;

        public Flock()
        {
            _boids = new List<Boid>(_numBoids);

            for (var i = 0; i < _numBoids; i++)
            {
                var boid = new Boid(x: _rand.Next(0, MainGame.ScreenWidth),
                                    y: _rand.Next(0, MainGame.ScreenHeight),
                                    flock: this);
                Add(boid);
            }
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
                new Vector2(boid._position.X, 0),
                new Vector2(boid._position.X, MainGame.ScreenHeight),
                new Vector2(0, boid._position.Y),
                new Vector2(MainGame.ScreenWidth, boid._position.Y)
            };
        }

        public void Update()
        {
            Task.Run(() =>
            {
                foreach (Boid boid in _boids)
                {
                    boid.Accelerate(FlockBehaviour.Avoidance(boid, _boids) * 1.5f);
                    boid.Accelerate(FlockBehaviour.AvoidPoints(boid, GetBorderPoints(boid)) * 5);
                    boid.Accelerate(FlockBehaviour.Alignment(boid, _boids) / 1.5f);
                    boid.Accelerate(FlockBehaviour.Cohesion(boid, _boids) / 3);
                    boid.Accelerate(boid._velocity / 7);

                    boid.Run();
                }
            });
        }
    }
}
