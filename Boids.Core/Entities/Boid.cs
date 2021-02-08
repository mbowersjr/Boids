using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core.Entities
{
    public class Boid
    {
        private static Random _rand = new Random();

        public static Texture2D BoidSprite;
        private Vector2 _boidSpriteOrigin = new Vector2(5f,5f);
        private const int _speed = 5;
        private const int _turnSpeed = 30 / _speed;

        public Vector2 _position { get; private set; }
        public Vector2 _cellPosition { get; private set; }

        public Vector2 _velocity = new Vector2(_rand.Next() * 2 - 1, _rand.Next() * 2 - 1);
        public Vector2 _acceleration = new Vector2();

        private Flock _flock;

        public Boid(int x, int y, Flock flock)
        {
            _position = new Vector2(x, y);
            _flock = flock;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture: BoidSprite,
                    position: _position,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: GetRotationRad(),
                    origin: _boidSpriteOrigin,
                    scale: Vector2.One,
                    effects: SpriteEffects.None,
                    layerDepth: 0f);

            sb.DrawLine(point1: _position,
                        point2: _position + _velocity * 3,
                        color: Color.Red,
                        thickness: 2);
        }

        public float GetRotationRad()
        {
            return (float)Math.Atan2(_velocity.Y, _velocity.X) + MathHelper.PiOver2;
        }

        public void Accelerate(Vector2 accel)
        {
            _acceleration += accel / _turnSpeed;
        }

        public void Run()
        {
            _velocity += _acceleration;
            _acceleration = Vector2.Zero;

            if (Math.Abs(_velocity.Length()) > _speed)
            {
                _velocity.Normalize();
                _velocity *= _speed;
            }

            _position += _velocity;
            _cellPosition = new Vector2(_position.X / MainGame.CellWidth, _position.Y / MainGame.CellHeight);

            Borders();
        }

        private void Borders()
        {
            if (_position.X < 0 || _position.X > MainGame.ScreenWidth ||
                _position.Y < 0 || _position.Y > MainGame.ScreenHeight)
            {
                _position = new Vector2(MainGame.ScreenWidth / 2, MainGame.ScreenHeight / 2);
            }
        }
    }
}
