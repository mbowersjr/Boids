using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Boids.Core.Gui.Views;
using ImGuiNET;

namespace Boids.Core.Gui
{
    public class GuiManager : IDisposable
    {
        private ImGuiRenderer _imGuiRenderer;

        private FlockListView _flockListView;
        private DebugConsoleView _debugConsoleView;

        private readonly Dictionary<string, GuiViewBase> _views = new Dictionary<string, GuiViewBase>();

        private readonly ILogger<GuiManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        private MainGame _game;

        public GuiManager(MainGame game, ILogger<GuiManager> logger, IServiceProvider serviceProvider)
        {
            _game = game;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Initialize()
        {
            _imGuiRenderer = ActivatorUtilities.CreateInstance<ImGuiRenderer>(_serviceProvider);
            //GuiTheme.ApplyTheme();
            _imGuiRenderer.RebuildFontAtlas();

            InitializeViews();
        }
        
        private void InitializeViews()
        {
            _flockListView = _serviceProvider.GetService<FlockListView>();
            AddView("FlockList", _flockListView);

            _debugConsoleView = _serviceProvider.GetService<DebugConsoleView>();
            AddView("DebugConsole", _debugConsoleView);
        }

        public void AddView(string name, GuiViewBase view)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("View name cannot be null or empty", nameof(name));
            }

            if (view == null)
            {
                ArgumentNullException.ThrowIfNull(view, nameof(view));
            }

            if (_views.ContainsKey(name))
            {
                throw new ArgumentException($"A view has already been registered with the name \"{name}\".", nameof(view));
            }
            
            _logger.LogInformation("Adding view {ViewName}", name);
            _views.Add(name, view);
        }

        public void RemoveView(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("View name cannot be null or empty", nameof(name));
            }

            if (!_views.ContainsKey(name))
            {
                throw new ArgumentException($"A view has not been registered with the name \"{name}\"", nameof(name));
            }

            _logger.LogInformation("Removing view {ViewName}", name);
            _views.Remove(name);
        }

        protected void LoadContent()
        {
            foreach (var view in _views)
            {
                _logger.LogTrace("View {ViewName} -> LoadContent", view.Key);
                view.Value.LoadContent();
            }
        }

        public void Draw(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);
            
            ShowDockSpace();

            foreach (var view in _views)
            {
                if (view.Value.IsVisible)
                {
                    // _logger.LogTrace("View {ViewName} -> LayoutView", view.Key);
                    view.Value.LayoutView();
                }
            }

            _imGuiRenderer.AfterLayout();
        }

        public uint DockSpaceId { get; set; }

        private void ShowDockSpace()
        {
            DockSpaceId = ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

            //var dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode;

            //var windowFlags = ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.None;
            //var viewport = ImGui.GetMainViewport();
            //ImGui.SetNextWindowPos(viewport.WorkPos);
            //ImGui.SetNextWindowSize(viewport.WorkSize);
            //ImGui.SetNextWindowViewport(viewport.ID);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            //windowFlags |= ImGuiWindowFlags.NoTitleBar | 
            //               ImGuiWindowFlags.NoCollapse | 
            //               ImGuiWindowFlags.NoResize |
            //               ImGuiWindowFlags.NoMove |
            //               ImGuiWindowFlags.NoBringToFrontOnFocus | 
            //               ImGuiWindowFlags.NoNavFocus |
            //               ImGuiWindowFlags.NoBackground;

            //ImGui.Begin("Dockspace", ref _dockspaceVisible, windowFlags);

            //var io = ImGui.GetIO();
            //if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
            //{
            //    _dockSpaceId = ImGui.GetID("GuiDockSpace");
            //    ImGui.DockSpace(_dockSpaceId, new Num.Vector2(0f, 0f), dockspaceFlags);
            //}

            ShowMainMenuBar();

            //ImGui.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var view in _views)
            {
                // _logger.LogTrace("View {ViewName} -> Update", view.Key);
                view.Value.Update(gameTime);
            }
        }

        private void ShowMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Exit", null, false, true))
                    {
                        _game.Exit();
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    ImGui.MenuItem("Flock List", "F", ref _flockListView.IsVisibleRef);
                    ImGui.MenuItem("Debug Console", "C", ref _debugConsoleView.IsVisibleRef);
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }


        #region IDisposable
        
        private bool _isDisposed;
        protected void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                foreach (var view in _views)
                {
                    _logger.LogInformation("Disposing view {ViewName}", view.Key);
                    view.Value.UnloadContent();
                }

                _views.Clear();
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        #endregion
    }
}
