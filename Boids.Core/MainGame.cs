using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Boids.Core.Entities;
using Boids.Core.Services;

namespace Boids.Core
{
    public class MainGame : Game
    {
        SpriteBatch _spriteBatch;
        public static GraphicsDeviceManager Graphics { get; private set; }
        
        public static PartitionGrid Grid => _partitionGrid;
        private static PartitionGrid _partitionGrid;
        
        private readonly IFlock _flock;
        private readonly ILogger<MainGame> _logger;
        private IOptionsMonitor<BoidsOptions> _optionsMonitor;
        private readonly IServiceProvider _services;
        public static BoidsOptions Options { get; set; }
        public static FastRandom Random { get; private set; } = new FastRandom();
        
        public MainGame(IFlock flock, PartitionGrid partitionGrid, IOptionsMonitor<BoidsOptions> optionsMonitor, IServiceProvider services, ILogger<MainGame> logger)
        {
            _partitionGrid = partitionGrid;
            _flock = flock;
            _optionsMonitor = optionsMonitor;
            _services = services;
            _logger = logger;

            Options = _optionsMonitor.CurrentValue;
            _optionsMonitor.OnChange(options =>
            {
                Options = options;
                
                _partitionGrid.Initialize();
                _flock.ResetFlock();
            });
            
            _logger.LogDebug("Applying graphics settings: {X} x {Y} VSync: {VSync}",
                Options.Graphics.Resolution.X, Options.Graphics.Resolution.Y, Options.Graphics.VSync);

            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = Options.Graphics.Resolution.X;
            Graphics.PreferredBackBufferHeight = Options.Graphics.Resolution.Y;
            Graphics.SynchronizeWithVerticalRetrace = Options.Graphics.VSync;
            Graphics.PreferMultiSampling = true;
            Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            Graphics.PreferredBackBufferWidth = Options.Graphics.Resolution.X;
            Graphics.PreferredBackBufferHeight = Options.Graphics.Resolution.Y;
            Graphics.ApplyChanges();

            CenterWindow();
            
            _partitionGrid.Initialize();
            _flock.ResetFlock();

            _flock.Paused = false;
        }
        
        private void CenterWindow()
        {
            var displayMode = Graphics.GraphicsDevice.DisplayMode;
            
            // Screen center
            var position = new Point(displayMode.Width / 2, displayMode.Height / 2);
            
            // Offset half window size
            position.X -= Window.ClientBounds.Width / 2;
            position.Y -= Window.ClientBounds.Height / 2;

            Window.Position = position;
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

            if (keyboardState.IsKeyDown(Keys.P)) 
                _flock.Paused = !_flock.Paused;

            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            _flock.Update(elapsedSeconds);
            _partitionGrid.UpdateActiveCells(_flock.Boids);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            _partitionGrid.Draw(gameTime);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate);
            _flock.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
