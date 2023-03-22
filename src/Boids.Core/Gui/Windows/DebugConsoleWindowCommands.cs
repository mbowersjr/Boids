using System;
using System.Linq;
using System.Reflection;
using Boids.Core.Configuration;
using Boids.Core.Entities;

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

        public static ConsoleCommandResult Config(ConsoleState state)
        {
            string output;

            var settingName = state.CurrentCommandArguments[0];
            var newValue = state.CurrentCommandArguments.ElementAtOrDefault(1);

            var settings = MainGame.Options;

            var settingProperty = GetSettingProperty(settings, settingName);
            
            if (settingProperty == null)
            {
                output = $"Setting \"{settingName}\" does not exist.";
            }
            else
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    var currentValue = settingProperty.GetValue(settings);
                    output = $"Setting \"{settingName}\" = {currentValue}";
                }
                else
                {
                    var convertedValue = Convert.ChangeType(newValue, settingProperty.PropertyType);
                    settingProperty.SetValue(settings, convertedValue);
                    output = $"Setting \"{settingName}\" set to {newValue}";

                    state.ExecuteCommand("RESET");
                }
            }

            return ConsoleCommandResult.Create(state.InputText, output);
        }

        private static PropertyInfo GetSettingProperty(BoidsOptions options, string settingName)
        {
            var properties = options.GetType().GetProperties();

            var property = properties.FirstOrDefault(x => x.Name.Equals(settingName, StringComparison.OrdinalIgnoreCase));

            return property;
        }
    }
}
