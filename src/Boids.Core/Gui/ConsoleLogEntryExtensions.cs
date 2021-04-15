using System;

namespace Boids.Core.Gui
{
    public static class ConsoleLogEntryExtensions
    {
        public static void LogTrace(this ConsoleState state, string text, params object[] args) => state.CreateLogEntry(text, ConsoleLogEntryLevel.Trace, args);
        public static void LogDebug(this ConsoleState state, string text, params object[] args) => state.CreateLogEntry(text, ConsoleLogEntryLevel.Debug, args);
        public static void LogInformation(this ConsoleState state, string text, params object[] args) => state.CreateLogEntry(text, ConsoleLogEntryLevel.Information, args);
        public static void LogWarning(this ConsoleState state, string text, params object[] args) => state.CreateLogEntry(text, ConsoleLogEntryLevel.Warning, args);
        public static void LogError(this ConsoleState state, string text, params object[] args) => state.CreateLogEntry(text, ConsoleLogEntryLevel.Error, args);
        public static void LogCritical(this ConsoleState state, string text, params object[] args) => state.CreateLogEntry(text, ConsoleLogEntryLevel.Critical, args);

        private static void CreateLogEntry(this ConsoleState state, string text, ConsoleLogEntryLevel entryLevel, params object[] args)
        {
            var entry = new ConsoleLogEntry(string.Format(text, args), entryLevel, DateTime.Now);
            state.LogEntries.Enqueue(entry);
        }
    }
}