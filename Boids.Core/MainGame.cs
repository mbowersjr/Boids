using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.Core.Entities;

namespace Boids.Core
{
    public class MainGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static int CellCountX = 32;
        public static int CellCountY = 24;

        public static int CellWidth = ScreenWidth / CellCountX;
        public static int CellHeight = ScreenHeight / CellCountY;

        Flock _flock;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.SynchronizeWithVerticalRetrace = false;

            this.IsMouseVisible = true;

            _graphics.ApplyChanges();

            InitGridLines();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _flock = new Flock();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Boid.BoidSprite = Content.Load<Texture2D>("Content/Boid");
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Q))
            {
                Exit();
            }

            base.Update(gameTime);

            _flock.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawGrid();

            _flock.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Tuple<Vector2,Vector2>[] _gridPoints;

        private void InitGridLines()
        {
            _gridPoints = new Tuple<Vector2,Vector2>[CellCountX + CellCountY];

            for (int x = 0; x < CellCountX; x++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(x * CellWidth, 0), new Vector2(x * CellWidth, ScreenHeight));
                _gridPoints[x] = points;
            }

            for (int y = 0; y < CellCountY; y++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(0, y * CellHeight), new Vector2(ScreenWidth, y * CellHeight));
                _gridPoints[CellCountX + y] = points;
            }
        }

        private void DrawGrid()
        {
            for (int i = 0; i < _gridPoints.Length; i++)
            {
                   _spriteBatch.DrawLine(point1: _gridPoints[i].Item1,
                                         point2: _gridPoints[i].Item2,
                                         color: Color.CornflowerBlue);
            }

            // for (int i = 0; i < ScreenWidth; i += CellWidth)
            // {
            //     _spriteBatch.DrawLine(point1: new Vector2(i, 0),
            //                           point2: new Vector2(i, ScreenHeight),
            //                           color: Color.CornflowerBlue);
            // }

            // for (int i = 0; i < ScreenHeight; i += CellHeight)
            // {
            //     _spriteBatch.DrawLine(point1: new Vector2(0, i),
            //                           point2: new Vector2(ScreenWidth, i),
            //                           color: Color.CornflowerBlue);
            // }
        }
    }
}
