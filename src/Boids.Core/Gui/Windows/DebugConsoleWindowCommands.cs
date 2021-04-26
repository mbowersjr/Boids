﻿namespace Boids.Core.Gui.Windows
{
    public static class DebugConsoleWindowCommands
    {
        public static ConsoleCommandResult Clear(ConsoleState state)
        {
            state.ClearHistory();
            
            return ConsoleCommandResult.EmptyResult;
        }

        public static ConsoleCommandResult Exit(ConsoleState state)
        {
            state.Game.Exit();
            
            return ConsoleCommandResult.Create("Exiting game ...");
        }

        public static ConsoleCommandResult Reset(ConsoleState state)
        {
            state.Game.Reset();
            
            return ConsoleCommandResult.Create("Simulation reset");
        }

        public static ConsoleCommandResult Help(ConsoleState state)
        {
            var output = "-------------------------\nBoids Flocking Simulation\n-------------------------";
            return ConsoleCommandResult.Create(output);
        }
        public static ConsoleCommandResult Pause(ConsoleState state)
        {
            state.Game.TogglePaused();
            
            var output = state.Game.IsPaused ? "Game paused" : "Game unpaused";
            return ConsoleCommandResult.Create(output);
        }
    }
}
