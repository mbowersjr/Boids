using System;
using System.Linq;
using System.Reflection;
using Boids.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boids.Core.Gui.Views
{
    public static class DebugConsoleViewCommands
    {
        public static DebugConsoleCommandResult Clear(DebugConsoleState state)
        {
            state.ClearHistory();
            return DebugConsoleCommandResult.Create(state.InputText, string.Empty);
        }

        public static DebugConsoleCommandResult Exit(DebugConsoleState state)
        {
            
            return DebugConsoleCommandResult.Create(state.InputText, "Exiting game ...");
        }

        public static DebugConsoleCommandResult Reset(DebugConsoleState state)
        {
            var game = Program.Host.Services.GetRequiredService<MainGame>();
            game.Reset();

            return DebugConsoleCommandResult.Create(state.InputText, "Game reset");
        }

        public static DebugConsoleCommandResult Help(DebugConsoleState state)
        {
            var output = 
@"------------------------- 
Boids Flocking Simulation
-------------------------";

            return DebugConsoleCommandResult.Create(state.InputText, output);
        }

        public static DebugConsoleCommandResult Pause(DebugConsoleState state)
        {
            var game = Program.Host.Services.GetRequiredService<MainGame>();
            game.IsPaused = !game.IsPaused;

            var output = game.IsPaused ? "Game paused" : "Game unpaused";
            return DebugConsoleCommandResult.Create(state.InputText, output);
        }

        public static DebugConsoleCommandResult Config(DebugConsoleState state)
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

            return DebugConsoleCommandResult.Create(state.InputText, output);
        }

        private static PropertyInfo GetSettingProperty(BoidsOptions options, string settingName)
        {
            var properties = options.GetType().GetProperties();

            var property = properties.FirstOrDefault(x => x.Name.Equals(settingName, StringComparison.OrdinalIgnoreCase));

            return property;
        }
    }
}
