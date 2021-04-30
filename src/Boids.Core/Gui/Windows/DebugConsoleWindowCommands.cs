namespace Boids.Core.Gui.Windows
{
    public static class DebugConsoleWindowCommands
    {
        public static ConsoleCommandResult Clear(ConsoleState state)
        {
            state.ClearHistory();
            return ConsoleCommandResult.Create(state.InputText, string.Empty);
        }

        public static ConsoleCommandResult Exit(ConsoleState state)
        {
            state.Game.Exit();
            return ConsoleCommandResult.Create(state.InputText, "Exiting game ...");
        }

        public static ConsoleCommandResult Reset(ConsoleState state)
        {
            state.Game.Reset();
            return ConsoleCommandResult.Create(state.InputText, "Simulation reset");
        }

        public static ConsoleCommandResult Help(ConsoleState state)
        {
            var output = "-------------------------\nBoids Flocking Simulation\n-------------------------";
            return ConsoleCommandResult.Create(state.InputText, output);
        }
        public static ConsoleCommandResult Pause(ConsoleState state)
        {
            state.Game.TogglePaused();
            
            var output = state.Game.IsPaused ? "Game paused" : "Game unpaused";
            return ConsoleCommandResult.Create(state.InputText, output);
        }
    }
}
