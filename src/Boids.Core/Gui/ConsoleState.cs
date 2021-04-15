using System;
using System.Collections.Generic;
using Boids.Core.Gui.Windows;

namespace Boids.Core.Gui
{
    public class ConsoleState
    {
        public string CurrentCommand { get; set; }
        public string[] CurrentCommandArguments { get; set; }
        public ConsoleCommandResult CurrentResult { get; set; }
        public string InputText { get; set; }
        public byte[] InputBuffer { get; set; }
        
        public const uint InputBufferSize = 1024;
        public Queue<ConsoleLogEntry> LogEntries { get; set; }
        public Queue<ConsoleLogEntry> CommandHistory { get; set; }
        public int HistoryPosition { get; set; }
        public bool AutoScroll { get; set; }
        public bool ScrollToButtom { get; set; }
        public MainGame Game { get; set; }

        private readonly Dictionary<string, Func<ConsoleState, ConsoleCommandResult>> _validCommands;

        private void AddValidCommand(string command, Func<ConsoleState, ConsoleCommandResult> commandFunc)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException(nameof(command));
            
            if (_validCommands.ContainsKey(command))
                throw new ArgumentException($"Command already exists in collection: '{command}'", nameof(command));

            _validCommands.Add(command, commandFunc);
        }

        public void InitializeValidCommands()
        {
            AddValidCommand("CLEAR", DebugConsoleWindowCommands.Clear);
            AddValidCommand("QUIT", DebugConsoleWindowCommands.Exit);
            AddValidCommand("EXIT", DebugConsoleWindowCommands.Exit);
            AddValidCommand("HELP", DebugConsoleWindowCommands.Help);
            AddValidCommand("RESET", DebugConsoleWindowCommands.Reset);
        }

        private void SetResult(string text, bool wasError)
        {
            CurrentResult = ConsoleCommandResult.Create(text, wasError);
        }
        
        public void SetCurrentCommand(string command, params string[] args)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException(nameof(command));
            
            CurrentCommand = command;
            CurrentCommandArguments = args;
        }

        public void ExecuteCommand(string command, params string[] args)
        {
            ConsoleCommandResult result;
            
            CurrentCommand = command;
            CurrentCommandArguments = args;
            
            if (!_validCommands.ContainsKey(CurrentCommand))
            {
                result = ConsoleCommandResult.Create($"Invalid command: '{CurrentCommand}'", true);
            }
            else
            {
                var validCommand = _validCommands[CurrentCommand];
                result = validCommand.Invoke(this);
            }
            
            CurrentResult = result;
        }
        
        public ConsoleState(MainGame game)
        {
            _validCommands = new Dictionary<string, Func<ConsoleState, ConsoleCommandResult>>();
            
            Game = game;
            InputBuffer = new byte[InputBufferSize];
            InputText = null;
            LogEntries = new Queue<ConsoleLogEntry>();
            CommandHistory = new Queue<ConsoleLogEntry>();
            HistoryPosition = -1;
            AutoScroll = true;
            ScrollToButtom = true;
        }

        public void ClearHistory()
        {
            LogEntries.Clear();
            CommandHistory.Clear();
            
            HistoryPosition = -1;
        }
    }
}