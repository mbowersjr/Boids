using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core;
using Boids.Core.Behaviors;
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
                _boidSpriteOrigin = new Vector2(_boidSprite.Width / 2f, _boidSprite.Height / 2f);
            }
        }

        private static Vector2 _boidSpriteOrigin;
        
        public const float MinSpawnVelocity = 5f;
        public const float MaxSpawnVelocity = 20f;
        public const float MaxVelocity = 30f;
        public const float MaxForce = 5f;

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        private float _rotation;
        private float Rotation { get => _rotation; set => _rotation = value; }

        private Vector2 _cellPosition;
        public Vector2 CellPosition { get => _cellPosition; private set => _cellPosition = value; }

        private Vector2 _velocity;
        public Vector2 Velocity 
        { 
            get => _velocity; 
            set => _velocity = value;
        }

        private Vector2 _acceleration;
        public Vector2 Acceleration
        {
            get => _acceleration;
            set => _acceleration = value;
        }

        public bool IsActive { get; set; }

        private readonly Flock _flock;

        public Boid(Flock flock)
        {
            _flock = flock;
        }

        private readonly Color _boidColor = new Color(Color.DarkGray, 1f);
        private readonly Color _avoidedPointLineColor = new Color(Color.Orange, 0.5f);
        private readonly Color _boidPropertiesTextColor = new Color(Color.Black, 0.5f);
        private readonly Color _boidDirectionLineColor = new Color(Color.Red, 0.75f);

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, sortMode: SpriteSortMode.Immediate);
            
            DrawBoid(spriteBatch);

            if (MainGame.Options.DisplayAvoidedPointLines)
            {
                DrawAvoidedPointLines(spriteBatch);
            }

            if (MainGame.Options.DisplayBoidProperties)
            {
                DrawBoidProperties(spriteBatch, spriteFont);
            }

            spriteBatch.End();
        }

        private void DrawBoid(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(texture: BoidSprite,
            //                  position: Position,
            //                  sourceRectangle: null,
            //                  color: BoidColor,
            //                  rotation: Rotation,
            //                  origin: _boidSpriteOrigin,
            //                  scale: 1f,
            //                  effects: SpriteEffects.None,
            //                  layerDepth: 0f);
            
            spriteBatch.DrawCircle(center: Position, 
                                   radius: 10f, 
                                   sides: 90, 
                                   color: _boidColor, 
                                   thickness: 2f, 
                                   layerDepth: 0f);

            spriteBatch.DrawLine(point: Position,
                                 angle: Rotation,
                                 length: 30f,
                                 color: _boidDirectionLineColor,
                                 thickness: 2f,
                                 layerDepth: 0f);
        }

        private IBehavior _avoidPointsBehavior = null;
        private void DrawAvoidedPointLines(SpriteBatch spriteBatch)
        {
            _avoidPointsBehavior ??= _flock.Behaviors.GetBehavior("AvoidPoints");
            
            var avoidedPoints = AvoidPointsBehavior.GetNearestBoundsPoints(this);
            
            foreach (var point in avoidedPoints)
            {
                var direction = Position - point;
                var distance = direction.Length();

                var lineColor = _avoidedPointLineColor;
                if (distance > 0f && distance < _avoidPointsBehavior.Radius)
                {
                    // More prominent line for avoided points within active range
                    lineColor = new Color(Color.Red, 0.75f);
                }
                
                spriteBatch.DrawLine(point1: Position,
                                     point2: point,
                                     color: lineColor,
                                     thickness: 1f,
                                     layerDepth: 0f);
            }
        }
        
        private readonly StringBuilder _sb = new StringBuilder();
        private void DrawBoidProperties(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            _sb.Clear();
            _sb.AppendFormat("P: {0:N3}, {1:N3}\n", Position.X, Position.Y);
            _sb.AppendFormat("V: {0:N3}, {1:N3}\n", Velocity.X, Velocity.Y);
            _sb.AppendFormat("R: {0:N3} rad.   \n", Rotation);

            var textSize = spriteFont.MeasureString(_sb);
            var textPosition = new Vector2(Position.X - textSize.X / 2f, Position.Y + 15f);

            spriteBatch.DrawString(spriteFont: spriteFont,
                                   text: _sb,
                                   position: textPosition,
                                   color: _boidPropertiesTextColor);
        }

        public void Reset()
        {
            IsActive = true;
            var quarterResolution = new Vector2(MainGame.Options.Graphics.Resolution.X / 4f, MainGame.Options.Graphics.Resolution.Y / 4f);
            var minX = quarterResolution.X * 1f;
            var maxX = quarterResolution.X * 3f;
            var minY = quarterResolution.Y * 1f;
            var maxY = quarterResolution.Y * 3f;
            
            Position = new Vector2(MainGame.Random.NextSingle(minX, maxX), MainGame.Random.NextSingle(minY, maxY));
            MainGame.Random.NextUnitVector(out _velocity);
            _velocity *= MainGame.Random.NextSingle(MinSpawnVelocity, MaxSpawnVelocity);
            Rotation = Velocity.ToRadians();
            Acceleration = Vector2.Zero;
        }
        
        public void Update(float elapsedSeconds)
        {
            if (!IsActive)
                return;

            Velocity += Acceleration * elapsedSeconds;
            Acceleration = Vector2.Zero;
            Velocity.Truncate(Boid.MaxVelocity);
            Rotation = Velocity.ToRadians();
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
