using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using Boids.Core.Entities;
using Boids.Core.Services;
using Boids.Core.Configuration;
using Boids.Core.Gui;

namespace Boids.Core
{
    public class MainGame : Game
    {
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;
        
        public static GraphicsDeviceManager Graphics { get; private set; }
        
        private readonly IFlock _flock;
        private readonly PartitionGrid _partitionGrid;
        private InputListenerService _inputService;
        private readonly IOptionsMonitor<BoidsOptions> _optionsMonitor;
        private readonly ILogger<MainGame> _logger;


        public static ViewportAdapter ViewportAdapter { get; private set; }
        
        public static BoidsOptions Options { get; set; }
        
        private GuiManager _guiManager;
        private IServiceProvider _serviceProvider;

        public MainGame(IFlock flock,
                        PartitionGrid partitionGrid,
                        IOptionsMonitor<BoidsOptions> optionsMonitor,
                        ILogger<MainGame> logger,
                        IServiceProvider serviceProvider)
        {

            _flock = flock;
            _optionsMonitor = optionsMonitor;
            _partitionGrid = partitionGrid;
            _logger = logger;
            _serviceProvider = serviceProvider;

            Options = _optionsMonitor.CurrentValue;
            _optionsMonitor.OnChange(OptionsMonitor_OnChanged);


            Graphics = new GraphicsDeviceManager(this);
            InitializeViewport();
            
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        private void OptionsMonitor_OnChanged(BoidsOptions options)
        {
            Options = options;
            InitializeViewport();

            Reset();
        }

        public void Reset()
        {
            _flock?.ResetFlock(GraphicsDevice.Viewport.Bounds.ToRectangleF(), Options);
        }
        
        protected override void Initialize()
        {
            _guiManager = _serviceProvider.GetService<GuiManager>();
            _guiManager.Initialize();

            _inputService = new InputListenerService(this);
            _inputService.Initialize();
            _inputService.GuiKeyboardListener.KeyPressed += InputService_OnKeyPressed;
   
            InitializeViewport();
            
            _partitionGrid.Initialize();
            
            _flock.ResetFlock(GraphicsDevice.Viewport.Bounds.ToRectangleF(), Options);
            
            base.Initialize();            
        }
        
        private void InitializeViewport()
        {
            if (Graphics == null)
                return;
            
            Graphics.PreferredBackBufferWidth = (int)(Options.Graphics.Resolution.X * Options.Graphics.Resolution.Scale);
            Graphics.PreferredBackBufferHeight = (int)(Options.Graphics.Resolution.Y * Options.Graphics.Resolution.Scale);
            Graphics.PreferMultiSampling = true;
            Graphics.ApplyChanges();

            ViewportAdapter = new WindowViewportAdapter(window: Window, Graphics.GraphicsDevice);
            ViewportAdapter.Reset();
            
            CenterWindow();

            using (_logger.BeginScope("Initialized viewport:"))
            {
                _logger.LogInformation("Resolution: {X} x {Y}", ViewportAdapter.ViewportWidth, ViewportAdapter.ViewportHeight);
                _logger.LogInformation("Window position: {X} x {Y}", Window.Position.X, Window.Position.Y);
                _logger.LogInformation("VSync: {VSync}", Graphics.SynchronizeWithVerticalRetrace);
            }
        }

        public bool IsPaused { get; set; }
        
        private void InputService_OnKeyPressed(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Escape || args.Key == Keys.Q)
            {
                LogInputCommand("Exit game");
                Exit();
            }

            if (args.Key == Keys.P)
            {
                LogInputCommand("Toggle pause game");
                IsPaused = !IsPaused;
            }

            if (args.Key == Keys.R)
            {
                LogInputCommand("Reset flock");
                _flock.ResetFlock(GraphicsDevice.Viewport.Bounds.ToRectangleF(), Options);
            }

            if (args.Key == Keys.F1)
            {
                LogInputCommand("Toggle displaying debug data");
                Options.DisplayDebugData = !Options.DisplayDebugData;
            }

            if (args.Key == Keys.OemComma && args.Modifiers.HasFlag(KeyboardModifiers.Control))
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
            ArgumentNullException.ThrowIfNull(file, nameof(file));
            
            if (!file.Exists)
                throw new FileNotFoundException(null, file.FullName);

            using (_openAppSettingsProcess ??= new Process())
            {
                _openAppSettingsProcess.StartInfo.FileName = "explorer";
                _openAppSettingsProcess.StartInfo.Arguments = $"\"{file.FullName}\"";
                _openAppSettingsProcess.EnableRaisingEvents = true;

                _logger.LogInformation("Opened appsettings.json with process: {ProcessName}", _openAppSettingsProcess.ProcessName);

                _openAppSettingsProcess.Exited += (sender, e) => {
                    _logger.LogInformation("Process used to open appsettings.json has exited with code {ExitCode}", ((Process)sender!).ExitCode);
                };

                return _openAppSettingsProcess.Start();
            }
        }
        
        private FileInfo GetAppSettingsFile()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyFile = new FileInfo(assemblyLocation);
            var appSettingsFile = assemblyFile.Directory!.EnumerateFiles("appsettings.json", SearchOption.TopDirectoryOnly).FirstOrDefault();

            if (appSettingsFile == null)
            {
                _logger.LogWarning("appsettings.json file not found in assembly directory {AssemblyDirectory}", assemblyFile.Directory!.FullName);
                return null;
            }

            _logger.LogInformation("Found appsettings.json file at {AppSettingsFilePath}", appSettingsFile.FullName);
            
            return appSettingsFile;
        }

        private void CenterWindow()
        {
            Window.Position = Graphics.GraphicsDevice.DisplayMode.TitleSafeArea.Center - Window.ClientBounds.Center;;
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

            _guiManager.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsPaused)
            {
                _flock.Update(gameTime);
                _partitionGrid.UpdateActiveCells(_flock.Boids);

            }
            
            _guiManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Options.Theme.BackgroundColor.Value);
            
            _partitionGrid.Draw(gameTime);
            
            _flock.Draw(gameTime, _spriteBatch, _spriteFont);

            _guiManager.Draw(gameTime);
        }

    }
}
