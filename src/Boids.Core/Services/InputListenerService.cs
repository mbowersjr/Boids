using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace Boids.Core.Services
{
    public class InputListenerComponentServiceLink
    {
        public InputListenerService Service { get; set; }
        public InputListenerComponent Component { get; set; }

        public InputListenerComponentServiceLink(InputListenerService service, InputListenerComponent component)
        {
            Service = service;
            Component = component;
        }
    }
    
    public class InputListenerService : IInputService, IDisposable
    {
        public KeyboardListener GuiKeyboardListener { get; private set; }
        public MouseListener GuiMouseListener { get; private set; }
        public GamePadListener GuiGamePadListener { get; private set; }
        public TouchListener GuiTouchListener { get; private set; }
        
        private MainGame _game;
        private readonly ILogger<InputListenerService> _logger;
        private InputListenerComponent _inputListenerComponent;
        
        public InputListenerService(ILogger<InputListenerService> logger)
        {
            _logger = logger;
            
            GuiMouseListener = new MouseListener(new MouseListenerSettings());
            GuiKeyboardListener = new KeyboardListener(new KeyboardListenerSettings());
            GuiGamePadListener = new GamePadListener(new GamePadListenerSettings());
            GuiTouchListener = new TouchListener(new TouchListenerSettings());
        }

        public void Initialize(MainGame game)
        {
            _game = game;
            
            _inputListenerComponent = new InputListenerComponent(_game, GuiKeyboardListener, GuiMouseListener, GuiGamePadListener, GuiTouchListener);
            _game.Components.Add(_inputListenerComponent);
            
            var componentServiceLink = new InputListenerComponentServiceLink(this, _inputListenerComponent);
            _game.Services.AddService<InputListenerComponentServiceLink>(componentServiceLink);
        }

        private int _scrollWheelValue = 0;
        private List<int> _keys = new List<int>();

        /*public void UpdateImGuiIO()
        {
            var io = ImGui.GetIO();

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            for (int i = 0; i < _keys.Count; i++)
            {
                io.KeysDown[_keys[i]] = keyboard.IsKeyDown((Keys)_keys[i]);
            }

            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new System.Numerics.Vector2(_game.GraphicsDevice.PresentationParameters.BackBufferWidth, _game.GraphicsDevice.PresentationParameters.BackBufferHeight);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

            io.MousePos = new System.Numerics.Vector2(mouse.X, mouse.Y);

            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

            var scrollDelta = mouse.ScrollWheelValue - _scrollWheelValue;
            io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
            _scrollWheelValue = mouse.ScrollWheelValue;
        }*/

/*
        public void InitImGuiIO()
        {
            var io = ImGui.GetIO();

            _keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
            _keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
            _keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
            _keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
            _keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
            _keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
            _keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
            _keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space);
            _keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
            _keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
            _keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
            _keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);
            
            // _game.Window.TextInput += (s, a) =>
            // {
            //     if (a.Character == '\t')
            //     {
            //         return;
            //     }
            //
            //     io.AddInputCharacter(a.Character);
            // };
            
            ImGui.GetIO().Fonts.AddFontDefault();
        }*/

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            
            if (disposing)
            {
                _inputListenerComponent = null;
                _game?.Dispose();
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