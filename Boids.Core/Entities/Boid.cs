using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core.Entities
{
    public class Boid
    {
        private static Random _rand = new Random();

        public static Texture2D BoidSprite;
        private static Vector2 _boidSpriteOrigin = new Vector2(5f,5f);

        private const int Speed = 5;
        private const int TurnSpeed = 30 / Speed;

        private Vector2 _position;
        public Vector2 Position { get => _position; set => _position = value; }

        private Vector2 _cellPosition;
        public Vector2 CellPosition { get => _cellPosition; set => _cellPosition = value; }

        private Vector2 _velocity;
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        private Vector2 _acceleration;
        public Vector2 Acceleration { get => _acceleration; set => _acceleration = value; }

        public bool IsActive { get; set; }

        private readonly Flock _flock;

        public Boid(Flock flock)
        {
            _flock = flock;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture: BoidSprite,
                    position: Position,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: GetRotationRad(),
                    origin: _boidSpriteOrigin,
                    scale: Vector2.One,
                    effects: SpriteEffects.None,
                    layerDepth: 0f);

            sb.DrawLine(point1: Position,
                        point2: Position + Vector2.Normalize(Velocity) * 10f,
                        color: Color.Red,
                        thickness: 2);
        }

        public float GetRotationRad()
        {
            return MathF.Atan2(Velocity.Y, Velocity.X) + MathHelper.PiOver2;
        }

        public void Accelerate(Vector2 accel)
        {
            if (!IsActive)
                return;

            Acceleration += accel / TurnSpeed;
        }

        public void Run()
        {
            if (!IsActive)
                return;

            Velocity += Acceleration;
            Acceleration = Vector2.Zero;

            if (Math.Abs(Velocity.Length()) > Speed)
            {
                Velocity.Normalize();
                Velocity *= Speed;
            }

            _position += Velocity;
            MainGame.Grid.GetCellPosition(ref _position, out _cellPosition);

            Borders();
        }

        private void Borders()
        {
            if (Position.X < 0 || Position.X > MainGame.Options.Graphics.Resolution.X ||
                Position.Y < 0 || Position.Y > MainGame.Options.Graphics.Resolution.Y)
            {
                _position = new Vector2(MainGame.Options.Graphics.Resolution.X / 2f, MainGame.Options.Graphics.Resolution.Y / 2f);
            }
        }
    }
}
