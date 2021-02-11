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

        public Vector2 Position { get; set; }
        public Vector2 CellPosition { get; set; }

        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        private readonly Flock _flock;

        public Boid(Vector2 position, Flock flock)
        {
            Position = position;
            Velocity = RandomStatic.NextUnitVector() * RandomStatic.NextSingle(1f, 2f);

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
            Acceleration += accel / TurnSpeed;
        }

        public void Run()
        {
            Velocity += Acceleration;
            Acceleration = Vector2.Zero;

            if (Math.Abs(Velocity.Length()) > Speed)
            {
                Velocity.Normalize();
                Velocity *= Speed;
            }

            Position += Velocity;
            CellPosition = MainGame.Grid.GetCellPosition(Position);

            Borders();
        }

        private void Borders()
        {
            if (Position.X < 0 || Position.X > MainGame.Options.Graphics.Resolution.X ||
                Position.Y < 0 || Position.Y > MainGame.Options.Graphics.Resolution.Y)
            {
                Position = new Vector2(MainGame.Options.Graphics.Resolution.X / 2f, MainGame.Options.Graphics.Resolution.Y / 2f);
            }
        }
    }
}
