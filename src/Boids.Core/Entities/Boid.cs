using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Boids.Core;
using Boids.Core.Behaviors;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Boids.Core.Entities
{
    public class Boid
    {
        private static Texture2D _boidSprite;
        public static Texture2D BoidSprite { get => _boidSprite; set => _boidSprite = value; }

        private static float BoidRadius => MainGame.Options.BoidRadius;

        private Vector2 _position;
        public Vector2 Position { get => _position; set => _position = value; }
        public ref Vector2 PositionRef => ref _position;

        private float _rotation;
        public float Rotation { get => _rotation; set => _rotation = value; }

        // private Point _cellPosition;
        // public Point CellPosition { get => _cellPosition; set => _cellPosition = value; }

        private Vector2 _velocity;
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        private Vector2 _acceleration;
        public Vector2 Acceleration { get => _acceleration; set => _acceleration = value; }

        public Vector2 PreviousAcceleration { get; set; }
        public bool IsActive { get; set; }

        private readonly Flock _flock;

        public Boid(Flock flock)
        {
            _flock = flock;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                              blendState: BlendState.NonPremultiplied,
                              sortMode: SpriteSortMode.Immediate,
                              transformMatrix: MainGame.ViewportAdapter.GetScaleMatrix());
            
            DrawBoid(spriteBatch);

            if (MainGame.Options.DisplayDebugData)
            {
                if (MainGame.Options.DisplayDistanceReferenceCircles)
                {
                    DrawDistanceReferenceCircles(spriteBatch);
                }
                if (MainGame.Options.AvoidedPointsDisplay.Enabled)
                {
                    DrawAvoidedPointLines(spriteBatch);
                }
                if (MainGame.Options.DisplayBoidPropertiesText)
                {
                    DrawBoidPropertiesText(spriteBatch, spriteFont);
                }
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
                                   radius: BoidRadius, 
                                   sides: 32, 
                                   color: MainGame.Options.Theme.BoidColor.Value, 
                                   thickness: 2f, 
                                   layerDepth: 1f);

            spriteBatch.DrawLine(point: Position,
                                 angle: Rotation,
                                 length: BoidRadius * 2,
                                 color: MainGame.Options.Theme.BoidDirectionLineColor.Value,
                                 thickness: 2f,
                                 layerDepth: 1f);

            if (MainGame.Options.DisplayDebugData)
            {
                spriteBatch.DrawLine(point1: Position,
                                     point2: Position + PreviousAcceleration,
                                     color: MainGame.Options.Theme.BoidForceLineColor.Value,
                                     thickness: 2f,
                                     layerDepth: 0f);
            }
        }

        private static readonly float[] _distances = new[] { 25f, 50f, 100f, 200f }; 
        private void DrawDistanceReferenceCircles(SpriteBatch spriteBatch)
        {
            foreach (var distance in _distances)
            {
                spriteBatch.DrawCircle(center: Position,
                                       radius: distance,
                                       sides: 32,
                                       color: MainGame.Options.Theme.AvoidedPointLineColor.Value,
                                       thickness: 1f,
                                       layerDepth: 1f);
            }
        }

        private IBehavior _avoidPointsBehavior;
        private void DrawAvoidedPointLines(SpriteBatch spriteBatch)
        {
            _avoidPointsBehavior ??= _flock.Behaviors.GetBehavior("AvoidPoints");
            
            Vector2[]  avoidedPoints = new Vector2[3];
            AvoidPointsBehavior.FindNearestBoundsPoints(this, ref avoidedPoints);
            
            foreach (var point in avoidedPoints)
            {
                var direction = Position - point;
                var distanceSquared = direction.LengthSquared();

                var pointIsActive = distanceSquared < _avoidPointsBehavior.RadiusSquared;

                Color lineColor;
                
                if (MainGame.Options.AvoidedPointsDisplay.ActivePoints && pointIsActive)
                {
                    lineColor = MainGame.Options.Theme.AvoidedPointActiveLineColor.Value;
                }
                else if (MainGame.Options.AvoidedPointsDisplay.NearestPoints)
                {
                    lineColor = MainGame.Options.Theme.AvoidedPointLineColor.Value;
                }
                else
                {
                    continue;
                }
                
                spriteBatch.DrawLine(point1: Position,
                                     point2: point,
                                     color: lineColor,
                                     thickness: 2f,
                                     layerDepth: 0f);
            }
        }
        
        private readonly StringBuilder _sb = new StringBuilder();
        private void DrawBoidPropertiesText(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            _sb.Clear();
            
            _sb.AppendFormat("P: {0:N3}, {1:N3}\n", Position.X, Position.Y);
            _sb.AppendFormat("V: {0:N3}, {1:N3}\n", Velocity.X, Velocity.Y);
            _sb.AppendFormat("A: {0:N3}, {1:N3}\n", Acceleration.X, Acceleration.Y);
            _sb.AppendFormat("R: {0:N3} rad.   \n", Rotation);

            var textSize = spriteFont.MeasureString(_sb);
            var textOrigin = Vector2.One; // new Vector2(textSize.X / 2f, 0f);
            var textPosition = new Vector2(Position.X - textSize.X / 2f, Position.Y + 15f);

            spriteBatch.DrawString(spriteFont: spriteFont,
                                   text: _sb,
                                   position: textPosition,
                                   color: MainGame.Options.Theme.BoidPropertiesTextColor.Value,
                                   rotation: 0f,
                                   origin: textOrigin,
                                   scale: 1f,
                                   effects: SpriteEffects.None,
                                   layerDepth: 0f);
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PreviousAcceleration = Acceleration;
            
            Velocity += Acceleration * elapsedSeconds;
            Velocity = Velocity.Truncate(MainGame.Options.Limits.MaxVelocity);
            
            Position += Velocity * elapsedSeconds;
            Rotation = Velocity.ToRadians();
            Acceleration = Vector2.Zero;

            WrapAroundViewportEdges();
        }

        private void WrapAroundViewportEdges()
        {
            if (_position.X < 0f)
            {
                _position.X = MainGame.ViewportAdapter.BoundingRectangle.Right;
            }
            else if (_position.X > MainGame.ViewportAdapter.BoundingRectangle.Right)
            {
                _position.X = 0f;
            }

            if (_position.Y < 0f)
            {
                _position.Y = MainGame.ViewportAdapter.BoundingRectangle.Bottom;
            }
            else if (_position.Y > MainGame.ViewportAdapter.BoundingRectangle.Bottom)
            {
                _position.Y = 0f;
            }
        }

        public void ApplyForce(Vector2 force)
        {
            // Implements Reynolds steering formula: steering force = desired velocity - current velocity
            // Reference: Nature of Code, chapter 6.3 (https://natureofcode.com/book/chapter-6-autonomous-agents/)

            var desiredVelocity = Velocity + force;
            desiredVelocity.Normalize();
            desiredVelocity *= MainGame.Options.Limits.MaxVelocity;

            var adjustment = desiredVelocity - Velocity;
            adjustment = adjustment.Truncate(MainGame.Options.Limits.MaxForce);
            
            Acceleration += adjustment;
        }

        // public void Seek(Vector2 target)
        // {
        //     var desiredPosition = target - _position;
        //     desiredPosition.Normalize();
        //     desiredPosition *= MainGame.Options.Limits.MaxVelocity;
        //     
        //     var adjustment = desiredPosition - Velocity;
        //     adjustment.Truncate(MainGame.Options.Limits.MaxForce);
        //
        //     ApplyForce(adjustment);
        // }

        // public void Flee(Vector2 from)
        // {
        //     Seek(from * -1);
        // }

        // public void Arrive(Vector2 target)
        // {
        //     var desired = target - _position;
        //
        //     var distance = desired.Length();
        //     desired.Normalize();
        //     
        //     if (distance < MainGame.Options.Limits.ArrivalDistance)
        //     {
        //         var scaledMagnitude = ScaleHelper.ScaleValue(val: distance,
        //                                                           fromMin: 0f, fromMax: MainGame.Options.Limits.ArrivalDistance, 
        //                                                           toMin:   0f, toMax:   MainGame.Options.Limits.MaxVelocity);
        //         desired *= scaledMagnitude;
        //     }
        //     else
        //     {
        //         desired *= MainGame.Options.Limits.MaxVelocity;
        //     }
        //
        //     var force = desired - _velocity;
        //     force.Truncate(MainGame.Options.Limits.MaxForce);
        //     
        //     ApplyForce(force);
        // }
    }
}
