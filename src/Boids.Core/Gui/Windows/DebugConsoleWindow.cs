using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Boids.Core.Services;
using Num = System.Numerics;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;

namespace Boids.Core.Gui.Windows
{
    public interface IDebugConsoleWindow
    {
        void Initialize();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void LoadContent();
        void UnloadContent();
        void ExecuteCommand(string inputText);
        
        bool IsVisible { get; set; }
        ConsoleState ConsoleState { get; }
        MainGame Game { get; }
    }
    
    public class DebugConsoleWindow : IDebugConsoleWindow
    {
        
        private ImGuiRenderer _imGuiRenderer;
        //private Texture2D _xnaTexture;
        //private IntPtr _imGuiTexture;
        private GraphicsDevice _graphics;
        
        private readonly string _windowTitle = "Debug Console";

        private readonly MainGame _game;
        public MainGame Game => _game;
        
        private readonly ConsoleState _state;
        public ConsoleState ConsoleState => _state;
        private InputListenerService _inputService;
        
        public bool IsVisible
        {
            get => _state.ConsoleWindowVisible;
            set => _state.ConsoleWindowVisible = value;
        }
        
        
        public DebugConsoleWindow(MainGame game)
        {
            _game = game;
            _state = new ConsoleState(this);
        }
        
        public void Initialize()
        {
            _graphics = _game.GraphicsDevice;
            
            _imGuiRenderer = new ImGuiRenderer(_game);
            _imGuiRenderer.RebuildFontAtlas();
            
            _state.InitializeValidCommands();

            var componentServiceLink = _game.Services.GetRequiredService<InputListenerComponentServiceLink>();
            _inputService = componentServiceLink.Service;
            _inputService.InitImGuiIO();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);

            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _inputService.UpdateImGuiIO();
                        
            if (_state.ConsoleWindowVisible)
            {
                DrawConsoleWindow();
            }
            
            _imGuiRenderer.AfterLayout();
        }

        public void LoadContent()
        {
            /*
            // Texture loading example

			// First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
			_xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
			{
				var red = (pixel % 300) / 2;
				return new Color(red, 1, 1);
			});

			// Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
			_imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);
            */
        }
        
        public void UnloadContent()
        {
        }
        

        private const string LogTextBoxId = "ScrollingRegion";
        private unsafe void DrawConsoleWindow()
        {
            ImGui.SetNextWindowSize(new Num.Vector2(400, 600), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(10, 10), ImGuiCond.FirstUseEver);

            // ImGui.Begin(_windowTitle, ref _state.ConsoleWindowVisible);
            
            if (!ImGui.Begin(_windowTitle, ref _state.ConsoleWindowVisible))
            {
                ImGui.End();
                return;
            }
            
            // As a specific feature guaranteed by the library, after calling Begin() the last Item represent the title bar.
            // So e.g. IsItemHovered() will return true when hovering the title bar.
            // Here we create a context menu only available from the title bar.
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Close"))
                {
                    _state.CloseWindow();
                }

                ImGui.EndPopup();
            }
            
            ImGui.TextWrapped("This is the debug console for interacting with the Boids simulation.");
            ImGui.TextWrapped("Enter 'HELP' for help.");

            if (ImGui.SmallButton("Clear"))
            {
                _state.ExecuteCommand("CLEAR");
            }
            ImGui.SameLine();
            if (ImGui.SmallButton("Close"))
            {
                _state.CloseWindow();
            }
            
            ImGui.Separator();

            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild(LogTextBoxId, new Num.Vector2(0, -footerHeight), false, ImGuiWindowFlags.HorizontalScrollbar);
            
            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.Selectable("Clear"))
                {
                    _state.ExecuteCommand("CLEAR");
                }
                ImGui.EndPopup();
            }


            /*
            Display every line as a separate entry so we can change their color or add custom widgets.
            If you only want raw text you can use ImGui::TextUnformatted(log.begin(), log.end());
        
            If you have thousands of entries this approach may be too inefficient and may require user - side clipping 
            to only process visible items. The clipper will automatically measure the height of your first item and then
            "seek" to display only items in the visible area.
            
            To use the clipper we can replace your standard loop:
                for (int i = 0; i < Items.Size; i++)
        
            With:
        
                ImGuiListClipper clipper;
                clipper.Begin(Items.Size);
                while (clipper.Step())
                    for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                        - That your items are evenly spaced(same height)
                        - That you have cheap random access to your elements(you can access them given their index,
                          without processing all the ones before)
            
            You cannot use this code as-is if a filter is active because it breaks the 'cheap random-access' property.
            We would need random-access on the post - filtered list.
        
            A typical application wanting coarse clipping and filtering may want to pre-compute an array of indices
            or offsets of items that passed the filtering test, recomputing this array when user changes the filter,
            and appending newly elements as they are inserted. This is left as a task to the user until we can manage
            to improve this example code!
        
            If your items are of variable height:
            - Split them into same height items would be simpler and facilitate random - seeking into your list. 
            - Consider using manual call to IsRectVisible() and skipping extraneous decoration from your items.
            */
            
             ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(4, 1));
            
            foreach (var entry in _state.LogEntries)
            {
                Num.Vector4 color;
                var hasColor = ConsoleState.GetLogEntryLevelColor(entry.EntryLevel, out color);

                if (hasColor)
                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                
                ImGui.TextUnformatted(entry.Text);

                if (hasColor)
                    ImGui.PopStyleColor();

                if (_state.ScrollToButtom || (_state.AutoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY()))
                    ImGui.SetScrollHereY(1.0f);
                
                _state.ScrollToButtom = false;
            }
            
            ImGui.PopStyleVar();
            ImGui.EndChild();
            ImGui.Separator();
                
            // Command line
            var reclaimFocus = false;
            
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

            ImGuiInputTextFlags inputTextFlags = 
                ImGuiInputTextFlags.EnterReturnsTrue | 
                ImGuiInputTextFlags.CallbackHistory | 
                ImGuiInputTextFlags.CallbackCompletion;
            
            if (ImGui.InputText("##inputText", ref _inputBuffer, _inputBufferSize, inputTextFlags, ImGuiInputTextFlags_Callback))
            {
                _state.InputText = _inputBuffer;
                ExecuteCommand(_state.InputText);

                _inputBuffer = string.Empty;
                _state.InputText = string.Empty;

                reclaimFocus = true;
            }

            _state.IsInputFocused = ImGui.IsItemFocused();
            
            // Auto-focus on window apparition
            ImGui.SetItemDefaultFocus();
                
            if (reclaimFocus)
            {
                // Auto focus previous widget
                ImGui.SetKeyboardFocusHere(-1);
            }
            
            ImGui.End();
        }

        private string _inputBuffer = string.Empty;
        private uint _inputBufferSize = 128;

        public void ExecuteCommand(string inputText)
        {
            _state.InputText = inputText;
            
            var words = _state.InputText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            var command = words.First();
            var args = words.Skip(1).ToArray();
            
            _state.ExecuteCommand(command, args);
            _state.CurrentResult.InputText = _state.InputText;
            
            if (_state.CurrentResult.WasError)
            {
                _state.LogError(_state.CurrentResult.OutputText);
            }
            else
            {
                _state.LogInformation(_state.CurrentResult.OutputText);
            }
        }

        private unsafe int ImGuiInputTextFlags_Callback(ImGuiInputTextCallbackData* data)
        {
            var dataPtr = new ImGuiInputTextCallbackDataPtr(data);
            
            switch (dataPtr.EventFlag)
            {
                case ImGuiInputTextFlags.CallbackCompletion:
                    return ImGuiInputTextFlags_CallbackCompletion(dataPtr);
                    
                case ImGuiInputTextFlags.CallbackHistory:
                    return ImGuiInputTextFlags_CallbackHistory(dataPtr);
                
                case ImGuiInputTextFlags.EnterReturnsTrue:
                    return ImGuiInputTextFlags_EnterReturnsTrue(dataPtr);
                
                default:
                    return 0;
            }
        }
        private int ImGuiInputTextFlags_CallbackCompletion(ImGuiInputTextCallbackDataPtr dataPtr)
        {
            if (dataPtr.EventKey == ImGuiKey.Enter)
            {
            }
            
            return 0;
        }
        private int ImGuiInputTextFlags_CallbackHistory(ImGuiInputTextCallbackDataPtr dataPtr)
        {
            if (dataPtr.EventKey == ImGuiKey.UpArrow)
            {
                var previousCommand = _state.GetPreviousCommand();
            
                if (previousCommand != null)
                {
                    dataPtr.DeleteChars(0, dataPtr.BufTextLen);
                    dataPtr.InsertChars(0, previousCommand.InputText);
                    dataPtr.SelectAll();
                }
            }
            
            return 0;
        }
        private int ImGuiInputTextFlags_EnterReturnsTrue(ImGuiInputTextCallbackDataPtr dataPtr)
        {
            var currentEventChar = (char)dataPtr.EventChar;
            return 0;
        }

        private void ImGuiLayout()
        {
            
        }
        
    }
}