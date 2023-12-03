﻿using System;
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

        private System.Numerics.Vector2 _numPosition;
        private Vector2 _position;

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _numPosition.X = value.X;
                _numPosition.Y = value.Y;
            }
        }

        public ref Vector2 PositionRef => ref _position;
        public ref System.Numerics.Vector2 NumPositionRef => ref _numPosition;

        private float _rotation;
        public float Rotation { get => _rotation; set => _rotation = value; }

        // private Point _cellPosition;
        // public Point CellPosition { get => _cellPosition; set => _cellPosition = value; }

        private System.Numerics.Vector2 _numVelocity;
        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                _numVelocity.X = value.X;
                _numVelocity.Y = value.Y;
            }
        }

        public ref Vector2 VelocityRef => ref _velocity;
        public ref System.Numerics.Vector2 NumVelocityRef => ref _numVelocity;

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
            //spriteBatch.Begin(samplerState: SamplerState.PointClamp,
            //                  blendState: BlendState.NonPremultiplied,
            //                  sortMode: SpriteSortMode.Immediate,
            //                  transformMatrix: MainGame.ViewportAdapter.GetScaleMatrix());

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

            //spriteBatch.End();
        }

        private float _layerDepth_BoidBody = 1f;
        private float _layerDepth_BoidDirectionLine = 1f;
        private float _layerDepth_BoidForceLine = 1f;
        private float _layerDepth_BoidPropertiesText = 1f;
        private float _layerDepth_AvoidedPointLine = 1f;

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
                                   layerDepth: _layerDepth_BoidBody);

            spriteBatch.DrawLine(point: Position,
                                 angle: Rotation,
                                 length: BoidRadius * 2,
                                 color: MainGame.Options.Theme.BoidDirectionLineColor.Value,
                                 thickness: 2f,
                                 layerDepth: _layerDepth_BoidDirectionLine);

            if (MainGame.Options.DisplayDebugData)
            {
                spriteBatch.DrawLine(point1: Position,
                                     point2: Position + PreviousAcceleration,
                                     color: MainGame.Options.Theme.BoidForceLineColor.Value,
                                     thickness: 2f,
                                     layerDepth: _layerDepth_BoidForceLine);
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
                                       layerDepth: _layerDepth_AvoidedPointLine);
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
                var distance = Vector2.Distance(Position, point);
                var pointIsActive = distance < _avoidPointsBehavior.Radius;

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
                                     layerDepth: _layerDepth_AvoidedPointLine);
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
            var textPosition = new Vector2(Position.X - textSize.X * 0.5f, Position.Y + BoidRadius * 1.5f);
            
            spriteBatch.DrawString(spriteFont: spriteFont,
                                   text: _sb,
                                   position: textPosition,
                                   color: MainGame.Options.Theme.BoidPropertiesTextColor.Value,                                   
                                   rotation: 0f,
                                   origin: Vector2.Zero,
                                   scale: 1f,
                                   effects: SpriteEffects.None,
                                   layerDepth: _layerDepth_BoidPropertiesText);
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PreviousAcceleration = Acceleration;
            
            Velocity += Acceleration * elapsedSeconds;
            Velocity.Limit(MainGame.Options.Limits.MaxVelocity);
            
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

        //private Vector2 Arrive(Vector2 target)
        //{
        //    var desired = target - _position;
        //    var dist = desired.Length();
            
        //    float mag;

        //    if (dist < MainGame.Options.Limits.ArrivalDistance)
        //    {
        //        mag = ScaleHelper.ScaleValue(dist, 0f, 100f, 0f, MainGame.Options.Limits.MaxVelocity);
        //    }
        //    else
        //    {
        //        mag = MainGame.Options.Limits.MaxVelocity;
        //    }

        //    desired.Normalize();
        //    desired *= mag;
            
        //    var steer = desired - _position;
        //    steer.Limit(MainGame.Options.Limits.MaxForce);
            
        //    _acceleration += steer;
        //}
      
        //public void Align(Vector2 target)
        //{
        //    var steer = target - _position;
        //    steer.Limit(MainGame.Options.Limits.MaxVelocity);
            
        //    ApplyForce_New(steer);
        //}

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
