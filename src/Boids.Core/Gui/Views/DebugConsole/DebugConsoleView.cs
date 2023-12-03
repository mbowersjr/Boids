using System;
using System.Linq;
using Boids.Core.Gui.Console;
using Boids.Core.Services;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Boids.Core.Gui.Views
{

    public class DebugConsoleView : GuiViewBase, IGuiWindow
    {
        public DebugConsoleState ConsoleState { get; private set; }
        
        public string WindowTitle { get; set; } = "Debug Console";

        public DebugConsoleView(DebugConsoleState consoleState, GuiManager guiManager, ILogger<DebugConsoleView> logger)
            : base(guiManager, logger)
        {
            ConsoleState = consoleState;
        }

        public override void Initialize()
        {
            ConsoleState.InitializeValidCommands();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        
        public override void LoadContent()
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
            base.LoadContent();
        }


        private const string LogTextBoxId = "ScrollingRegion";

        public override unsafe void LayoutView()
        {
            ImGui.SetNextWindowSize(new Num.Vector2(400f, 600f), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(10f, 10f), ImGuiCond.FirstUseEver);

            if (!ImGui.Begin(WindowTitle, ref IsVisibleRef))
            {
                ImGui.End();
                return;
            }

            // As a specific feature guaranteed by the library, after calling Begin() the last Item represent the title bar.
            // So e.g. IsItemHovered() will return true when hovering the title bar.
            // Here we create a context menu only available from the title bar.
            if (ImGui.BeginPopupContextItem())
            {
                ImGui.MenuItem("Close", null, ref IsVisibleRef);
                ImGui.EndPopup();
            }

            ImGui.TextWrapped("This is the debug console for interacting with the Boids simulation.");
            ImGui.TextWrapped("Enter 'HELP' for help.");

            if (ImGui.SmallButton("Clear"))
            {
                ConsoleState.ExecuteCommand("CLEAR");
            }

            ImGui.SameLine();
            if (ImGui.SmallButton("Close"))
            {
                IsVisibleRef = false;
            }

            ImGui.Separator();

            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild(
                str_id: LogTextBoxId,
                size: new Num.Vector2(0, -footerHeight),
                child_flags: ImGuiChildFlags.None,
                window_flags: ImGuiWindowFlags.HorizontalScrollbar);

            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.Selectable("Clear"))
                {
                    ConsoleState.ExecuteCommand("CLEAR");
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

            foreach (var entry in ConsoleState.LogEntries)
            {
                Num.Vector4 color;
                var hasColor = DebugConsoleState.GetLogEntryLevelColor(entry.EntryLevel, out color);

                if (hasColor)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                }

                ImGui.TextUnformatted(entry.Text);

                if (hasColor)
                {
                    ImGui.PopStyleColor();
                }

                if (ConsoleState.ScrollToButtom || ConsoleState.AutoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                {
                    ImGui.SetScrollHereY(1.0f);
                }

                ConsoleState.ScrollToButtom = false;
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

            if (ImGui.InputText(
                    label: "##inputText",
                    input: ref ConsoleState.InputTextRef,
                    maxLength: _inputBufferSize,
                    flags: inputTextFlags,
                    callback: ImGuiInputTextFlags_Callback))
            {
                ExecuteCommand(ConsoleState.InputText);

                ConsoleState.InputText = string.Empty;

                reclaimFocus = true;
            }

            ConsoleState.IsInputFocused = ImGui.IsItemFocused();

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
            ConsoleState.InputText = inputText;

            var words = ConsoleState.InputText.Split(' ',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var command = words.First();
            var args = words.Skip(1).ToArray();

            ConsoleState.ExecuteCommand(command, args);
            ConsoleState.CurrentResult.InputText = ConsoleState.InputText;

            if (ConsoleState.CurrentResult.WasError)
            {
                ConsoleState.LogError(ConsoleState.CurrentResult.OutputText);
            }
            else
            {
                ConsoleState.LogInformation(ConsoleState.CurrentResult.OutputText);
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
                var previousCommand = ConsoleState.GetPreviousCommand();

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
            var currentEventChar = (char) dataPtr.EventChar;
            return 0;
        }
    }
}
