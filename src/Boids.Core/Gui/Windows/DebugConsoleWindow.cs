using System;
using System.Linq;
using System.Text;
using Num = System.Numerics;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core.Gui.Windows
{
    public class DebugConsoleWindow : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private readonly ConsoleState _state;
        
        private IImGuiRenderer _imGuiRenderer;
        private Texture2D _xnaTexture;
        private IntPtr _imGuiTexture;
        private GraphicsDeviceManager _graphics;

        public bool IsVisible
        {
            get => _isConsoleWindowOpen;
            set => _isConsoleWindowOpen = value;
        }
        
        public DebugConsoleWindow(MainGame game) 
            : base(game)
        {
            _state = new ConsoleState(game);
        }
        
        public override void Initialize()
        {
            _imGuiRenderer = new ImGuiRenderer(this.Game);
            _imGuiRenderer.RebuildFontAtlas();
            
            _state.InitializeValidCommands();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);
            
            ImGuiLayout();
            
            _imGuiRenderer.AfterLayout();
            
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            _xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });

            _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);
            
            base.LoadContent();
        }
        
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        
        private string _consoleWindowTitle = "Debug Console";
        private bool _isConsoleWindowOpen = false;
        
        protected virtual void DrawConsoleWindow()
        {
            ImGui.SetNextWindowSize(new Num.Vector2(520, 600), ImGuiCond.FirstUseEver);
            
            if (!ImGui.Begin(_consoleWindowTitle, ref _isConsoleWindowOpen))
            {
                ImGui.End();
                return;
            }
            
            // As a specific feature guaranteed by the library, after calling Begin() the last Item represent the title bar.
            // So e.g. IsItemHovered() will return true when hovering the title bar.
            // Here we create a context menu only available from the title bar.
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Close Console")) 
                    _isConsoleWindowOpen = false;

                ImGui.EndPopup();
            }
            
            ImGui.TextWrapped("This is the debug console for interacting with the Boids simulation.");
            ImGui.TextWrapped("Enter 'HELP' for help.");

            if (ImGui.SmallButton("Clear"))
            {
                ClearConsole();
            }

            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild("ScrollingRegion", new System.Numerics.Vector2(0, -footerHeight), false, ImGuiWindowFlags.HorizontalScrollbar);
            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.Selectable("Clear"))
                {
                    _state.ExecuteCommand("CLEAR");
                    ImGui.EndPopup();
                }
            }
            
            
            // Display every line as a separate entry so we can change their color or add custom widgets.
            // If you only want raw text you can use ImGui::TextUnformatted(log.begin(), log.end());
            // NB- if you have thousands of entries this approach may be too inefficient and may require user-side clipping
            // to only process visible items. The clipper will automatically measure the height of your first item and then
            // "seek" to display only items in the visible area.
            // To use the clipper we can replace your standard loop:
            //      for (int i = 0; i < Items.Size; i++)
            //   With:
            //      ImGuiListClipper clipper;
            //      clipper.Begin(Items.Size);
            //      while (clipper.Step())
            //         for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            // - That your items are evenly spaced (same height)
            // - That you have cheap random access to your elements (you can access them given their index,
            //   without processing all the ones before)
            // You cannot this code as-is if a filter is active because it breaks the 'cheap random-access' property.
            // We would need random-access on the post-filtered list.
            // A typical application wanting coarse clipping and filtering may want to pre-compute an array of indices
            // or offsets of items that passed the filtering test, recomputing this array when user changes the filter,
            // and appending newly elements as they are inserted. This is left as a task to the user until we can manage
            // to improve this example code!
            // If your items are of variable height:
            // - Split them into same height items would be simpler and facilitate random-seeking into your list.
            // - Consider using manual call to IsRectVisible() and skipping extraneous decoration from your items.
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(4, 1));
            
            foreach (var entry in _state.LogEntries)
            {
                Num.Vector4 color;
                var hasColor = GetLogEntryLevelColor(entry.EntryLevel, out color);

                if (hasColor) 
                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                
                ImGui.TextUnformatted(entry.Text);
                
                if (hasColor)
                    ImGui.PopStyleColor();
                
                if (_state.ScrollToButtom || (_state.AutoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY()))
                    ImGui.SetScrollHereY(1.0f);
                _state.ScrollToButtom = false;
                
                ImGui.PopStyleVar();
                ImGui.EndChild();
                ImGui.Separator();
                
                
                // Command line
                var reclaimFocus = false;
                ImGuiInputTextFlags inputTextFlags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory;
                if (ImGui.InputText("Input", _state.InputBuffer, ConsoleState.InputBufferSize, inputTextFlags))
                {
                    var inputText = Encoding.UTF8.GetString(_state.InputBuffer);
                    
                    _state.InputText = inputText.Trim();
                    if (!string.IsNullOrEmpty(_state.InputText))
                    {
                        ExecuteCommand(_state);
                    }
                    
                    reclaimFocus = true;
                }

                // Auto-focus on window apparition
                ImGui.SetItemDefaultFocus();
                
                if (reclaimFocus)
                    ImGui.SetKeyboardFocusHere(-1); // Auto focus previous widget

                ImGui.End();
            }
        }

        private void ExecuteCommand(ConsoleState state)
        {
            var words = state.InputText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            var command = words.First();
            var args = words.Skip(1).ToArray();
            
            _state.ExecuteCommand(command, args);
        }

        private unsafe int InputTextEditCallback(ImGuiInputTextCallbackData* data)
        {
            throw new NotImplementedException();
        }

        private static readonly Num.Vector4 DefaultLogEntryColor = new Num.Vector4(0.04f, 0.04f, 0.04f, 1.0f);
        private static bool GetLogEntryLevelColor(ConsoleLogEntryLevel level, out Num.Vector4 color)
        {
            if (level == ConsoleLogEntryLevel.Error || level == ConsoleLogEntryLevel.Critical)
            {
                color = new Num.Vector4(1.0f, 0.4f, 0.4f, 1.0f);
                return true;
            }
            
            if (level == ConsoleLogEntryLevel.Warning)
            {
                color = new Num.Vector4(1.0f, 0.8f, 0.2f, 1.0f);
                return true;

            }
            
            if (level == ConsoleLogEntryLevel.Debug || level == ConsoleLogEntryLevel.Trace)
            {
                color = new Num.Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                return true;
            }
            
            color = DefaultLogEntryColor;
            return false;
        }

        private void ClearConsole()
        {

        }
        
        protected virtual void ImGuiLayout()
        {

            DrawConsoleWindow();

            //// 1. Show a simple window
            //// Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            //{
            //    ImGui.Text("Hello, world!");
            //    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
            //    ImGui.ColorEdit3("clear color", ref clear_color);
            //    if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
            //    if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
            //    ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

            //    ImGui.InputText("Text input", _textBuffer, 100);

            //    ImGui.Text("Texture sample");
            //    ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
            //}

            //// 2. Show another simple window, this time using an explicit Begin/End pair
            //if (show_another_window)
            //{
            //    ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
            //    ImGui.Begin("Another Window", ref show_another_window);
            //    ImGui.Text("Hello");
            //    ImGui.End();
            //}

            //// 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            //if (show_test_window)
            //{
            //    ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
            //    ImGui.ShowDemoWindow(ref show_test_window);
            //}
        }
        
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for(var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint( pixel );
            }

            //set the color
            texture.SetData( data );

            return texture;
        }
    }
}