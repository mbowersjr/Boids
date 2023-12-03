using System.Collections.Generic;
using Boids.Core.Gui.Views;

namespace Boids.Core.Gui.Console
{
    public class DebugConsoleLoggerOptions
    {
        public int EventId { get; set; }

        public List<DebugConsoleLogEntryLevel> LogLevels { get; set; } =
            new List<DebugConsoleLogEntryLevel>()
            {
                DebugConsoleLogEntryLevel.Information,
                DebugConsoleLogEntryLevel.Warning,
                DebugConsoleLogEntryLevel.Error
            };
    }
}