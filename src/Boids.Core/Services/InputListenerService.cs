using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Boids.Core.Services
{
    public class InputListenerService : IInputService, IDisposable
    {
        public KeyboardListener GuiKeyboardListener { get; private set; }
        public MouseListener GuiMouseListener { get; private set; }
        public GamePadListener GuiGamePadListener { get; private set; }
        public TouchListener GuiTouchListener { get; private set; }
        
        private MainGame _game;
        private InputListenerComponent _inputListenerComponent;
        
        public InputListenerService(MainGame game)
        {
            _game = game;
        }

        public void Initialize()
        {
            GuiMouseListener = new MouseListener(new MouseListenerSettings());
            GuiKeyboardListener = new KeyboardListener(new KeyboardListenerSettings());
            GuiGamePadListener = new GamePadListener(new GamePadListenerSettings());
            GuiTouchListener = new TouchListener(new TouchListenerSettings());

            _inputListenerComponent = new InputListenerComponent(_game, GuiKeyboardListener, GuiMouseListener, GuiGamePadListener, GuiTouchListener);
            _game.Components.Add(_inputListenerComponent);
        }
        
        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            
            if (disposing)
            {
                _inputListenerComponent?.Dispose();
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}