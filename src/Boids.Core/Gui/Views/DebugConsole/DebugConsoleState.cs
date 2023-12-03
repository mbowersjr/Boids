using System;
using System.Collections.Generic;
using Boids.Core.Gui.Console;

namespace Boids.Core.Gui.Views
{
    public class DebugConsoleState
    {
        public string CurrentCommand { get; set; }
        public string[] CurrentCommandArguments { get; set; }
        public DebugConsoleCommandResult CurrentResult { get; set; }
        
        private string _inputText = string.Empty;
        public string InputText 
        { 
            get => _inputText;
            set => _inputText = value;
        }
        public ref string InputTextRef => ref _inputText;

        public const uint InputBufferSize = 1024;
        public byte[] InputBuffer { get; set; } = new byte[InputBufferSize];
        public Queue<DebugConsoleLogEntry> LogEntries { get; set; } = new Queue<DebugConsoleLogEntry>();
        public List<DebugConsoleCommandResult> CommandHistory { get; set; } = new List<DebugConsoleCommandResult>();
        public bool IsInputFocused { get; set; }
        public int HistoryPosition { get; set; } = -1;
        public bool AutoScroll { get; set; } = true;
        public bool ScrollToButtom { get; set; } = true;
        
        private bool _consoleWindowVisible;
        public ref bool ConsoleWindowVisible => ref _consoleWindowVisible;

        private readonly Dictionary<string, Func<DebugConsoleState, DebugConsoleCommandResult>> _validCommands = 
            new Dictionary<string, Func<DebugConsoleState, DebugConsoleCommandResult>>(StringComparer.OrdinalIgnoreCase);

        private void AddValidCommand(string command, Func<DebugConsoleState, DebugConsoleCommandResult> commandFunc)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Value cannot be null or empty.", nameof(command));

            if (_validCommands.ContainsKey(command))
                throw new ArgumentException($"Command already exists in collection: '{command}'", nameof(command));

            _validCommands.Add(command, commandFunc);
        }

        public void InitializeValidCommands()
        {
            AddValidCommand("CLEAR", DebugConsoleViewCommands.Clear);
            AddValidCommand("QUIT", DebugConsoleViewCommands.Exit);
            AddValidCommand("EXIT", DebugConsoleViewCommands.Exit);
            AddValidCommand("HELP", DebugConsoleViewCommands.Help);
            AddValidCommand("RESET", DebugConsoleViewCommands.Reset);

            AddValidCommand("CONFIG", DebugConsoleViewCommands.Config);
        }

        public void SetCurrentCommand(string command, params string[] args)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException("Command cannot be null or empty", nameof(command));

            CurrentCommand = command;
            CurrentCommandArguments = args;
        }

        public void ExecuteCommand(string command, params string[] args)
        {
            DebugConsoleCommandResult result;

            CurrentCommand = command;
            CurrentCommandArguments = args;

            Func<DebugConsoleState, DebugConsoleCommandResult> commandFunc;
            if (!_validCommands.TryGetValue(CurrentCommand, out commandFunc))
            {
                result = new DebugConsoleCommandResult(InputText, $"Invalid command: '{CurrentCommand}'", true);
            }
            else
            {
                result = commandFunc.Invoke(this);
            }

            CurrentResult = result;
            CommandHistory.Add(CurrentResult);
        }


        public DebugConsoleState()
        {
        }

        public DebugConsoleCommandResult GetPreviousCommand()
        {
            if (CommandHistory.Count == 0)
                return null;

            if (HistoryPosition > 0)
            {
                HistoryPosition--;
            }
            else
            {
                HistoryPosition = CommandHistory.Count - 1;
            }

            var previousCommand = CommandHistory[HistoryPosition];
            return previousCommand;
        }

        public void ClearHistory()
        {
            LogEntries.Clear();

            CommandHistory.Clear();
            HistoryPosition = -1;
        }


        private static readonly System.Numerics.Vector4 DefaultLogEntryColor = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.0f);

        public static bool GetLogEntryLevelColor(DebugConsoleLogEntryLevel level, out System.Numerics.Vector4 color)
        {
            if (level == DebugConsoleLogEntryLevel.Error || level == DebugConsoleLogEntryLevel.Critical)
            {
                color = new System.Numerics.Vector4(1.0f, 0.4f, 0.4f, 1.0f);
                return true;
            }

            if (level == DebugConsoleLogEntryLevel.Warning)
            {
                color = new System.Numerics.Vector4(1.0f, 0.8f, 0.2f, 1.0f);
                return true;
            }

            if (level == DebugConsoleLogEntryLevel.Debug || level == DebugConsoleLogEntryLevel.Trace)
            {
                color = new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                return true;
            }

            color = DefaultLogEntryColor;
            return false;
        }
    }
}