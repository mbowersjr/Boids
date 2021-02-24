using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Boids.Core.Services
{
    public interface IInputListenerService  : IDisposable
    {
        MouseListener MouseListener { get; }
        KeyboardListener KeyboardListener { get; }
        GamePadListener GamePadListener { get; }

        void Initialize(MainGame game);
    }
    
    public class InputListenerService : IInputListenerService
    {
        public MouseListener MouseListener { get; private set; }
        public KeyboardListener KeyboardListener { get; private set; }
        public GamePadListener GamePadListener { get; private set; }
        private MainGame _game;
        private readonly ILogger<IInputListenerService> _logger;
        
        public InputListenerService(ILogger<InputListenerService> logger)
        {
            _logger = logger;
            
            MouseListener = new MouseListener(new MouseListenerSettings());
            KeyboardListener = new KeyboardListener(new KeyboardListenerSettings());
            GamePadListener = new GamePadListener(new GamePadListenerSettings());
        }
        
        public void Initialize(MainGame game)
        {
            _game = game;
            
            var inputListenerComponent = new InputListenerComponent(_game, KeyboardListener, MouseListener, GamePadListener); 
            _game.Components.Add(inputListenerComponent);
            
            // KeyboardListener.KeyPressed += OnKeyPressed;
        }

        private void UnregisterEventHandlers()
        {
            // KeyboardListener.KeyPressed -= OnKeyPressed;
        }
        
        // private void OnKeyPressed(object sender, KeyboardEventArgs e)
        // {
        // }

        // public void Update(GameTime gameTime)
        // {
        //     // _keyboardState = Keyboard.GetState();
        //     // _mouseState = Mouse.GetState();
        //     //
        //     // if (_keyboardState.IsKeyDown(Keys.Escape) || 
        //     //     _keyboardState.IsKeyDown(Keys.Q))
        //     //     ExitInput?.Invoke(this, CreateGameInputEventArgs());
        //     //
        //     // if (_keyboardState.IsKeyDown(Keys.P)) 
        //     //     PauseInput?.Invoke(this, CreateGameInputEventArgs());
        //     //
        //     // if (_keyboardState.IsKeyDown(Keys.R)) 
        //     //     ResetInput?.Invoke(this, CreateGameInputEventArgs());
        //     //
        //     // if (_keyboardState.IsKeyDown(Keys.D)) 
        //     //     ToggleDebugInput?.Invoke(this, CreateGameInputEventArgs());
        //     //
        //     //
        // }
        
        
        private bool _isDisposed;
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;
            
            if (disposing)
            {
                UnregisterEventHandlers();
                _game?.Dispose();
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}