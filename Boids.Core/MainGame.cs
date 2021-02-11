using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
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

        private PartitionGridRenderer _gridRenderer;

        public static PartitionGrid Grid { get; private set; }

        Flock _flock;

        private readonly ILogger<MainGame> _logger;
        public static BoidsOptions Options { get; private set; }

        public MainGame(BoidsOptions options, ILogger<MainGame> logger = null)
        {
            _logger = logger ?? NullLogger<MainGame>.Instance;
            Options = options;

            _logger.LogInformation("Initializing graphics device. BackBuffer resolution: {X} x {Y} VSync: {VSync}", Options.Graphics.Resolution.X, Options.Graphics.Resolution.Y, Options.Graphics.VSync);

            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = Options.Graphics.Resolution.X;
            _graphics.PreferredBackBufferHeight = Options.Graphics.Resolution.Y;
            _graphics.SynchronizeWithVerticalRetrace = Options.Graphics.VSync;

            this.IsMouseVisible = true;

            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";


            _gridRenderer = new PartitionGridRenderer(GraphicsDevice, Options.PartitionGrid.CellsX, Options.PartitionGrid.CellsY);
            Grid = _gridRenderer.Grid;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _gridRenderer.Initialize();

            _flock = new Flock();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Boid.BoidSprite = Content.Load<Texture2D>("Images/Boid");
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

            _gridRenderer.Draw();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _flock.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
