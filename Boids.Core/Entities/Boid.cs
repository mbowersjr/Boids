using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core;
using MonoGame.Extended;

namespace Boids.Core.Entities
{
    public class Boid
    {
        private static Texture2D _boidSprite;
        public static Texture2D BoidSprite
        {
            get => _boidSprite;
            set
            {
                _boidSprite = value;
                BoidSpriteOrigin = new Vector2(_boidSprite.Width / 2f, _boidSprite.Height / 2f);
            }
        }

        private static Vector2 _boidSpriteOrigin;
        private static Vector2 BoidSpriteOrigin { get => _boidSpriteOrigin; set => _boidSpriteOrigin = value; }

        private const float MinSpawnVelocity = 2f;
        private const float MaxSpawnVelocity = 10f;
        private const float MaxVelocity = 15f;
        private const float TurningVelocity = 10f;

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y))
                    throw new Exception("Attempted to set Position as invalid float.NaN");
                _position = value;
            }
        }

        private float _rotation;
        private float Rotation { get => _rotation; set => _rotation = value; }

        private Vector2 _cellPosition;
        public Vector2 CellPosition { get => _cellPosition; private set => _cellPosition = value; }

        private Vector2 _velocity;
        public Vector2 Velocity 
        { 
            get => _velocity; 
            set
            {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y))
                    throw new Exception("Attempted to set Velocity as invalid float.NaN");
                _velocity = value;
            } 
        }

        private Vector2 _acceleration;
        public Vector2 Acceleration
        {
            get => _acceleration;
            set
            {
                if (float.IsNaN(value.X) || float.IsNaN(value.Y))
                    throw new Exception("Attempted to set Acceleration as invalid float.NaN");
                _acceleration = value;
            }
        }

        public bool IsActive { get; set; }

        private readonly Flock _flock;

        public Boid(Flock flock)
        {
            _flock = flock;
        }

        private Vector2 GetMovementLineEndPoint()
        {
            return Position + Vector2.Normalize(Velocity) * Acceleration;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture: BoidSprite,
                    position: Position,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: Rotation,
                    origin: BoidSpriteOrigin,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0f);

            sb.DrawLine(point1: Position,
                        point2: GetMovementLineEndPoint(),
                        color: Color.Red,
                        thickness: 2f,
                        layerDepth: 0f);
        }

        public void ApplyForce(Vector2 force)
        {
            Acceleration += force;
        }

        public void Reset()
        {
            IsActive = true;
            Position = new Vector2(MainGame.Random.NextSingle(0f, MainGame.Options.Graphics.Resolution.X),
                                   MainGame.Random.NextSingle(0f, MainGame.Options.Graphics.Resolution.Y));
            MainGame.Random.NextUnitVector(out _velocity);
            _velocity *= MainGame.Random.NextSingle(MinSpawnVelocity, MaxSpawnVelocity);
            Rotation = Velocity.ToRadians() - MathHelper.PiOver2;
            Acceleration = Vector2.Zero;
        }
        
        public void Update(float elapsedSeconds)
        {
            if (!IsActive)
                return;

            Velocity += Acceleration * elapsedSeconds;
            Acceleration = Vector2.Zero;
            Velocity.Truncate(MaxVelocity);
            Rotation = Velocity.ToRadians() ;
            Position += Velocity * elapsedSeconds;
            CellPosition = MainGame.Grid.GetCellPosition(this);

            if (Position.X < 0f || Position.X > MainGame.Options.Graphics.Resolution.X ||
                Position.Y < 0f || Position.Y > MainGame.Options.Graphics.Resolution.Y)
            {
                Reset();
            }
        }
    }
}
