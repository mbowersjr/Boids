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
                //_boidSpriteOrigin = new Vector2(_boidSprite.Width / 2f, _boidSprite.Height / 2f);
            }
        }

        
        
        public const float MinSpawnVelocity = 3f;
        public const float MaxSpawnVelocity = 10f;
        public const float MaxVelocity = 10f;
        public const float MaxForce = 10f;

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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied, sortMode: SpriteSortMode.Immediate);
            
            DrawBoid(spriteBatch);

            if (MainGame.Options.DisplayAvoidedPointLines)
            {
                DrawAvoidedPointLines(spriteBatch);
            }

            if (MainGame.Options.DisplayBoidPropertiesText)
            {
                DrawBoidPropertiesText(spriteBatch, spriteFont);
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
                                   sides: 32, 
                                   color: MainGame.Options.Theme.BoidColor.Value, 
                                   thickness: 2f, 
                                   layerDepth: 0f);

            spriteBatch.DrawLine(point: Position,
                                 angle: Rotation,
                                 length: 30f,
                                 color: MainGame.Options.Theme.BoidDirectionLineColor.Value,
                                 thickness: 2f,
                                 layerDepth: 0f);
        }

        private IBehavior _avoidPointsBehavior;
        private void DrawAvoidedPointLines(SpriteBatch spriteBatch)
        {
            _avoidPointsBehavior ??= _flock.Behaviors.GetBehavior("AvoidPoints");
            
            Vector2[]  avoidedPoints = new Vector2[3];
            AvoidPointsBehavior.UpdateNearestAvoidedPoints(this, ref avoidedPoints);
            
            foreach (var point in avoidedPoints)
            {
                var direction = Position - point;
                var distance = direction.Length();

                var pointIsActive = distance > 0f && distance < _avoidPointsBehavior.Radius;
                
                var lineColor = pointIsActive 
                    ? MainGame.Options.Theme.AvoidedPointActiveLineColor.Value 
                    : MainGame.Options.Theme.AvoidedPointLineColor.Value;

                if (pointIsActive && MainGame.Options.DisplayAvoidedPointLines)
                {
                    spriteBatch.DrawLine(point1: Position,
                                         point2: point,
                                         color: lineColor,
                                         thickness: 1f,
                                         layerDepth: 0f);    
                }
            }
        }
        
        private readonly StringBuilder _sb = new StringBuilder();
        private void DrawBoidPropertiesText(SpriteBatch spriteBatch, SpriteFont spriteFont)
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
                                   color: MainGame.Options.Theme.BoidPropertiesTextColor.Value);
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

            Velocity += Acceleration.Truncate(Boid.MaxForce) * elapsedSeconds;
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
