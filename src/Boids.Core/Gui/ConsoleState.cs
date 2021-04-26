﻿using System;
using System.Collections.Generic;
using Num = System.Numerics;
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
        public bool IsInputFocused { get; set; }
        public int HistoryPosition { get; set; }
        public bool AutoScroll { get; set; }
        public bool ScrollToButtom { get; set; }
        
        private bool _consoleWindowVisible = false;
        public ref bool ConsoleWindowVisible => ref _consoleWindowVisible;

        private readonly DebugConsoleWindow _debugConsoleWindow;
        
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
        
        public ConsoleState(DebugConsoleWindow debugConsoleWindow)
        {
            _validCommands = new Dictionary<string, Func<ConsoleState, ConsoleCommandResult>>();
            _debugConsoleWindow = debugConsoleWindow;
            Game = _debugConsoleWindow.Game;
            InputBuffer = new byte[InputBufferSize];
            InputText = null;
            LogEntries = new Queue<ConsoleLogEntry>();
            CommandHistory = new Queue<ConsoleLogEntry>();
            HistoryPosition = -1;
            AutoScroll = true;
            ScrollToButtom = true;
        }

        public void ShowWindow() => _consoleWindowVisible = true;
        public void CloseWindow() => _consoleWindowVisible = false;

        public void ClearHistory()
        {
            LogEntries.Clear();
            CommandHistory.Clear();
            
            HistoryPosition = -1;
        }
        
        
        private static readonly Num.Vector4 DefaultLogEntryColor = new Num.Vector4(0.04f, 0.04f, 0.04f, 1.0f);
        
        public static bool GetLogEntryLevelColor(ConsoleLogEntryLevel level, out Num.Vector4 color)
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
    }
}