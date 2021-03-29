using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Microsoft.Extensions.Configuration.Json;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

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
        private readonly IOptionsMonitor<BoidsOptions> _optionsMonitor;
        private readonly IInputListenerService _inputListener;
        // ReSharper disable once NotAccessedField.Local
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
                
                InitializeViewport();
                _partitionGrid.Initialize();
                _flock.ResetFlock();
            });
            
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            
            Graphics = new GraphicsDeviceManager(this);
            var virtualWidth = (int)(Options.Graphics.Resolution.X * Options.Graphics.Resolution.Scale);
            var virtualHeight = (int)(Options.Graphics.Resolution.Y * Options.Graphics.Resolution.Scale);
            Graphics.PreferredBackBufferWidth = virtualWidth;
            Graphics.PreferredBackBufferHeight = virtualHeight;
            Graphics.ApplyChanges();
            
            InitializeViewport();
        }

        protected override void Initialize()
        {
            base.Initialize();

            InitializeViewport();
            
            _inputListener.Initialize(this);
            RegisterInputHandlers();
            _partitionGrid.Initialize();
            _flock.ResetFlock();

            _flock.Paused = false;
        }

        private void InitializeViewport()
        {
            var virtualWidth = (int)(Options.Graphics.Resolution.X * Options.Graphics.Resolution.Scale);
            var virtualHeight = (int)(Options.Graphics.Resolution.Y * Options.Graphics.Resolution.Scale);
            
            Graphics.PreferredBackBufferWidth = virtualWidth;
            Graphics.PreferredBackBufferHeight = virtualHeight;
            Graphics.ApplyChanges();

            ViewportAdapter = new BoxingViewportAdapter(window: Window,
                graphicsDevice: Graphics.GraphicsDevice,
                virtualWidth: virtualWidth,
                virtualHeight: virtualHeight);

            ViewportAdapter.Reset();
            
            CenterWindow();

            using (_logger.BeginScope("Initialized viewport:"))
            {
                _logger.LogInformation("Resolution: {X} x {Y}", ViewportAdapter.ViewportWidth, ViewportAdapter.ViewportHeight);
                _logger.LogInformation("Virtual resolution: {X} x {Y}", ViewportAdapter.VirtualWidth, ViewportAdapter.VirtualHeight);
                _logger.LogInformation("Window position: {X} x {Y}", Window.Position.X, Window.Position.Y);
                _logger.LogInformation("VSync: {VSync}", Graphics.SynchronizeWithVerticalRetrace);
            }
        }

        private void RegisterInputHandlers()
        {
            _inputListener.KeyboardListener.KeyPressed += InputListener_OnKeyPressed;
        }
        
        private void InputListener_OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            if (e.Key == Keys.Escape || e.Key == Keys.Q)
            {
                LogInputCommand("Exit game");
                Exit();
            }

            if (e.Key == Keys.P)
            {
                LogInputCommand("Toggle pause game");
                _flock.Paused = !_flock.Paused;
            }

            if (e.Key == Keys.R)
            {
                LogInputCommand("Reset flock");
                _flock?.ResetFlock();
            }

            if (e.Key == Keys.F1)
            {
                LogInputCommand("Toggle displaying debug data");
                Options.DisplayDebugData = !Options.DisplayDebugData;
            }

            if (e.Key == Keys.OemComma && e.Modifiers.HasFlag(KeyboardModifiers.Control))
            {
                LogInputCommand("Open appsettings.json");
                var appSettingsFile = GetAppSettingsFile();
                OpenFileInExternalEditor(appSettingsFile);
            }
        }

        private void LogInputCommand(string commandDescription)
        {
            if (string.IsNullOrEmpty(commandDescription))
                return;
            
            _logger.LogDebug("Input command: {CommandDescription}", commandDescription);
        }

        private Process _openAppSettingsProcess;
        
        private bool OpenFileInExternalEditor(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            
            if (!file.Exists)
                throw new FileNotFoundException("File does not exist", file.FullName);

            if (_openAppSettingsProcess == null) 
                _openAppSettingsProcess = new Process();

            _openAppSettingsProcess.StartInfo.FileName = "explorer";
            _openAppSettingsProcess.StartInfo.Arguments = $"\"{file.FullName}\"";
            _openAppSettingsProcess.EnableRaisingEvents = true;
            
            _logger.LogInformation("Opened appsettings.json with process: {ProcessName}", _openAppSettingsProcess.ProcessName);

            _openAppSettingsProcess.Exited += (_, _) =>
            {
                _logger.LogInformation("Process used to open appsettings.json has exited with code {ExitCode}", _openAppSettingsProcess.ExitCode);
            };
            
            return _openAppSettingsProcess.Start();
        }
        
        private FileInfo GetAppSettingsFile()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyFile = new FileInfo(assemblyLocation);
            var assemblyDirectory = assemblyFile.Directory;

            if (assemblyDirectory == null)
            {
                throw new InvalidOperationException();
            }

            var appSettingsFile = assemblyDirectory.GetFiles("appsettings.json", SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (appSettingsFile == null)
            {
                _logger.LogWarning("appsettings.json file not found in assembly directory {AssemblyDirectory}", assemblyDirectory.FullName);
                return null;
            }

            _logger.LogDebug("Located appsettings.json file at: {AppSettingsFilePath}", appSettingsFile.FullName);
            return appSettingsFile;
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
            _flock.Update(gameTime);
            // _partitionGrid.UpdateActiveCells(_flock.Boids);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(MainGame.Options.Theme.BackgroundColor.Value);

            _partitionGrid.Draw(gameTime);

            _flock.Draw(gameTime, _spriteBatch, _spriteFont);
        }
    }
}
