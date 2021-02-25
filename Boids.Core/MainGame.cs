using System;
using System.Threading;
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
using Boids.Core.Configuration;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace Boids.Core
{
    public class MainGame : Game
    {
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;
        public static GraphicsDeviceManager Graphics { get; private set; }
        
        public static PartitionGrid Grid => _partitionGrid;
        private static PartitionGrid _partitionGrid;
        
        private readonly IFlock _flock;
        private readonly ILogger<MainGame> _logger;
        private IOptionsMonitor<BoidsOptions> _optionsMonitor;        
        private readonly IInputListenerService _inputListener;
        private readonly CancellationToken _cancellationToken;
        public static ViewportAdapter ViewportAdapter { get; private set; }
        
        public static BoidsOptions Options { get; set; }
        public static FastRandom Random { get; private set; } = new FastRandom();
        
        public MainGame(IFlock flock, 
                        PartitionGrid partitionGrid, 
                        IInputListenerService inputListener,
                        IOptionsMonitor<BoidsOptions> optionsMonitor, 
                        ILogger<MainGame> logger,
                        CancellationToken cancellationToken)
        {
            _flock = flock;
            _partitionGrid = partitionGrid;
            _inputListener = inputListener;
            _optionsMonitor = optionsMonitor;
            _logger = logger;
            _cancellationToken = cancellationToken;
            
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

            var virtualWidth = (int)(Options.Graphics.Resolution.X * Options.Graphics.Resolution.Scale);
            var virtualHeight = (int)(Options.Graphics.Resolution.Y * Options.Graphics.Resolution.Scale);
            
            Graphics.PreferredBackBufferWidth = virtualWidth;
            Graphics.PreferredBackBufferHeight = virtualHeight;
            Graphics.ApplyChanges();

            ViewportAdapter = new BoxingViewportAdapter(window: Window,
                                                         graphicsDevice: Graphics.GraphicsDevice,
                                                         virtualWidth: Options.Graphics.Resolution.X,
                                                         virtualHeight: Options.Graphics.Resolution.Y);
            
            CenterWindow();
            
            _inputListener.Initialize(this);
            RegisterInputHandlers();
            _partitionGrid.Initialize();
            _flock.ResetFlock();

            _flock.Paused = false;
        }

        private void RegisterInputHandlers()
        {
            _inputListener.KeyboardListener.KeyPressed += InputListener_OnKeyPressed;
        }
        
        private void InputListener_OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            if (e.Key == Keys.Escape || e.Key == Keys.Q)
            {
                Exit();
            }

            if (e.Key == Keys.P)
            {
                _flock.Paused = !_flock.Paused;
            }

            if (e.Key == Keys.R)
            {
                _flock?.ResetFlock();
            }

            if (e.Key == Keys.OemTilde)
            {
                Options.DisplayDebugData = !Options.DisplayDebugData;
            }
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
            
            _spriteFont = Content.Load<SpriteFont>("Fonts/FixedWidth");
            Boid.BoidSprite = Content.Load<Texture2D>("Images/Boid_32x32");
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
            Boid.BoidSprite.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {            
            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            _flock.Update(elapsedSeconds);
            _partitionGrid.UpdateActiveCells(_flock.Boids);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(MainGame.Options.Theme.BackgroundColor.Value);

            _partitionGrid.Draw(gameTime, ViewportAdapter);

            _flock.Draw(gameTime, _spriteBatch, _spriteFont, ViewportAdapter);
        }
    }
}
