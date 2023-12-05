using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Num = System.Numerics;
using Microsoft.Xna.Framework;
using Boids.Core.Gui.Views;
using Microsoft.Xna.Framework.Content;
using ImGuiNET;

namespace Boids.Core.Gui
{
    public class GuiManager : IDisposable
    {
        private ImGuiRenderer _imGuiRenderer;

        private readonly Dictionary<string, IGuiView> _views = new Dictionary<string, IGuiView>();

        private readonly MainGame _game;
        private readonly IGuiViewIdProvider _viewIdProvider;
        private readonly IGuiFontProvider _fontProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GuiManager> _logger;

        private bool _dockSpaceVisible = true;
        private uint _dockSpaceId;
        private bool _isFullscreen = true;
        private bool _windowPadding = false;

        private ImGuiViewportPtr _mainViewport;

        public static float TextBaseWidth = ImGui.CalcTextSize("A").X;
        public static float TextBaseHeight = ImGui.GetTextLineHeightWithSpacing();
        
        private static class DebuggingWindows
        {
            public static bool Metrics = false;
            public static bool IDStackTool = false;
            public static bool DebugLog = false;
            public static bool StyleEditor = false;
            public static bool StyleSelector = false;
            public static bool FontSelector = false;
        }
        

        private ImGuiDockNodeFlags _dockNodeFlags = ImGuiDockNodeFlags.PassthruCentralNode;
        private ImGuiWindowFlags _windowFlags = ImGuiWindowFlags.NoBackground ;


        public GuiManager(MainGame game, IGuiViewIdProvider viewIdProvider, IGuiFontProvider fontProvider, IServiceProvider serviceProvider, ILogger<GuiManager> logger)
        {
            _game = game;
            _viewIdProvider = viewIdProvider;
            _fontProvider = fontProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;

        }

        private bool _isInitialized;

        public void Initialize()
        {
            _imGuiRenderer = ActivatorUtilities.CreateInstance<ImGuiRenderer>(_serviceProvider);
            
            // GuiTheme.ApplyTheme();
            _imGuiRenderer.RebuildFontAtlas();

            InitializeViews();

            _isInitialized = true;
        }
        
        private void InitializeViews()
        {
            var views = _serviceProvider.GetServices<IGuiView>();
            foreach (var view in views)
            {
                AddView(view);
            }
        }

        public string GenerateId<T>(T view) where T : class, IGuiView
        {
            var id = _viewIdProvider.GenerateId(view);
            return id;
        }

        public void AddView<T>(T view) where T : class, IGuiView
        {
            ArgumentNullException.ThrowIfNull(view, nameof(view));
            
            if (string.IsNullOrEmpty(view.Name))
                throw new ArgumentException("Name propery of view instance cannot be null or empty.", nameof(view));

            if (string.IsNullOrEmpty(view.Id))
                throw new ArgumentException("Id property of view instance cannot be null or empty.", nameof(view));
            
            if (_views.TryGetValue(view.Id, out IGuiView existingView))
            {
                _logger.LogWarning("View \"{ViewName}\" (Type: {ViewType}) already registered with ID \"{ViewId}\"", existingView.Name, existingView.GetType().Name, view.Id);
                throw new ArgumentException($"Another view instance has already been registered with the ID \"{view.Id}\"", nameof(view));
            }

            _views.Add(view.Id, view);
        }

        public void RemoveView(string id)
        {
            if (_views.TryGetValue(id, out IGuiView view))
            {
                _logger.LogInformation("Removing view \"{ViewName}\" (Type: {ViewType}, Id: {ViewId})", view.Name, view.GetType().Name, view.Id);
               _views.Remove(id);
            }
        }

        protected void LoadContent()
        {
            foreach (var view in _views)
            {
                _logger.LogTrace("View {ViewName} -> LoadContent", view.Value.Name);
                view.Value.LoadContent();
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isInitialized)
                return;

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            _imGuiRenderer.BeforeLayout(gameTime);

            ShowDockSpace();

            ShowMainMenuBar();
            if (DebuggingWindows.Metrics) ImGui.ShowMetricsWindow(ref DebuggingWindows.Metrics);
            if (DebuggingWindows.DebugLog) ImGui.ShowDebugLogWindow(ref DebuggingWindows.DebugLog);
            if (DebuggingWindows.IDStackTool) ImGui.ShowIDStackToolWindow(ref DebuggingWindows.IDStackTool);
            //if (DebuggingWindows.StyleEditor) ImGui.ShowStyleEditor(ImGui.GetStyle());
            //if (DebuggingWindows.StyleSelector) ImGui.ShowStyleSelector("Style Selector");
            //if (DebuggingWindows.FontSelector) ImGui.ShowFontSelector("Font Selector");

            // ImGui.SetNextWindowDockID((_dockSpaceId));
            ImGui.SetNextWindowDockID(_dockSpaceId, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Num.Vector2(400f, 600f), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(0f, 0f), ImGuiCond.FirstUseEver);
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

        private void ShowDockSpace()
        {
            /*
            if (_isFullscreen)
            {
                var viewport = ImGui.GetMainViewport();

                ImGui.SetNextWindowPos(viewport.WorkPos);
                ImGui.SetNextWindowSize(viewport.WorkSize);
                ImGui.SetNextWindowViewport(viewport.ID);

                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

                _windowFlags |= ImGuiWindowFlags.NoTitleBar | 
                                ImGuiWindowFlags.NoCollapse | 
                                ImGuiWindowFlags.NoResize |
                                ImGuiWindowFlags.NoMove |
                                ImGuiWindowFlags.NoBringToFrontOnFocus | 
                                ImGuiWindowFlags.NoNavFocus;
            }
            else
            {
                _dockSpaceFlags &= ~ImGuiDockNodeFlags.PassthruCentralNode;
            }

            if (_dockSpaceFlags.HasFlag(ImGuiDockNodeFlags.PassthruCentralNode))
                _windowFlags |= ImGuiWindowFlags.NoBackground;

            if (!_windowPadding) 
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Num.Vector2.Zero);

            ImGui.Begin("Dockspace", ref _dockSpaceVisible, _windowFlags);

            if (!_windowPadding)
                ImGui.PopStyleVar();

            if (_isFullscreen)
                ImGui.PopStyleVar(2);

            var io = ImGui.GetIO();
            if ((io.ConfigFlags & ImGuiConfigFlags.DockingEnable) != 0)
            {
                _dockSpaceId = ImGui.GetID("GuiDockSpace");
                ImGui.DockSpace(_dockSpaceId, Num.Vector2.Zero, _dockSpaceFlags);
            }
            else
            {
                ShowDockingDisabledMessage();
            }

            ShowMainMenuBar();

            ImGui.End();
        */

            _mainViewport = ImGui.GetMainViewport();
            _dockSpaceId = ImGui.DockSpaceOverViewport(_mainViewport, _dockNodeFlags);
        }

        private void ShowDockingDisabledMessage()
        {
            var io = ImGui.GetIO();

            ImGui.Text("ERROR: Docking is not enabled.");
            if (ImGui.Button("Enable Docking", new Num.Vector2(0f, 0f)))
                io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
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
                    if (ImGui.MenuItem("Close", null, false, _dockSpaceVisible))
                        _dockSpaceVisible = false;
                    
                    ImGui.Separator();

                    if (ImGui.MenuItem("Exit", null, false, true))
                        ExitGame();

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Options"))
                {
                    ImGui.MenuItem("Fullscreen", null, ref _isFullscreen);
                    ImGui.MenuItem("Window Padding", null, ref _windowPadding);
                    ImGui.Separator();

                    if (ImGui.MenuItem("No Docking Over Central Node", null, (_dockNodeFlags & ImGuiDockNodeFlags.NoDockingOverCentralNode) != 0))
                        _dockNodeFlags ^= ImGuiDockNodeFlags.NoDockingOverCentralNode;
                    
                    if (ImGui.MenuItem("Passthru Central Node", null, (_dockNodeFlags & ImGuiDockNodeFlags.PassthruCentralNode) != 0, _isFullscreen))
                        _dockNodeFlags ^= ImGuiDockNodeFlags.PassthruCentralNode;

                    if (ImGui.MenuItem("Auto-Hide Tab Bar", null, (_dockNodeFlags & ImGuiDockNodeFlags.AutoHideTabBar) != 0))
                        _dockNodeFlags ^= ImGuiDockNodeFlags.AutoHideTabBar;

                    if (ImGui.MenuItem("No Docking Split", null, (_dockNodeFlags & ImGuiDockNodeFlags.NoDockingSplit) != 0))
                        _dockNodeFlags ^= ImGuiDockNodeFlags.NoDockingSplit;

                    if (ImGui.MenuItem("No Resize", null, (_dockNodeFlags & ImGuiDockNodeFlags.NoResize) != 0))
                        _dockNodeFlags ^= ImGuiDockNodeFlags.NoResize;

                    if (ImGui.MenuItem("No Undocking", null, (_dockNodeFlags & ImGuiDockNodeFlags.NoUndocking) != 0))
                        _dockNodeFlags ^= ImGuiDockNodeFlags.NoUndocking;

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Tools"))
                {
                    ImGui.MenuItem("Metrics/Debugger", null, ref DebuggingWindows.Metrics, true);
                    ImGui.MenuItem("ID Stack Tool", null, ref DebuggingWindows.IDStackTool, true);
                    ImGui.MenuItem("Debug Log", null, ref DebuggingWindows.DebugLog, true);
                    ImGui.MenuItem("Style Editor", null, ref DebuggingWindows.StyleEditor, true);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    foreach (var view in _views.Values)
                    {
                        ImGui.MenuItem(view.Name, null, ref view.IsVisibleRef);
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }

        private void ExitGame()
        {
            _game.Exit();
        }

        #region Fonts

        
        private int _setFontStack = 0;

        public void PushFont(GuiFontStyle fontStyle)
        {
            var font = _fontProvider.GetFontPtr(fontStyle);
            ImGui.PushFont(font);
            _setFontStack++;
        }

        public void PopFont()
        {
            if (_setFontStack == 0)
                return;

            ImGui.PopFont();
            _setFontStack--;
        }


        #endregion

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
