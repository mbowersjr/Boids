using System;

namespace Boids.Core.Gui
{
    public static class ConsoleLogEntryExtensions
    {
        public static void LogTrace(this ConsoleState state, string text, params object[] args) => state.LogEntry(text, ConsoleLogEntryLevel.Trace, args);
        public static void LogDebug(this ConsoleState state, string text, params object[] args) => state.LogEntry(text, ConsoleLogEntryLevel.Debug, args);
        public static void LogInformation(this ConsoleState state, string text, params object[] args) => state.LogEntry(text, ConsoleLogEntryLevel.Information, args);
        public static void LogWarning(this ConsoleState state, string text, params object[] args) => state.LogEntry(text, ConsoleLogEntryLevel.Warning, args);
        public static void LogError(this ConsoleState state, string text, params object[] args) => state.LogEntry(text, ConsoleLogEntryLevel.Error, args);
        public static void LogCritical(this ConsoleState state, string text, params object[] args) => state.LogEntry(text, ConsoleLogEntryLevel.Critical, args);
        
        public static void LogEntry(this ConsoleState state, string text, ConsoleLogEntryLevel entryLevel, params object[] args)
        {
            var entry = new ConsoleLogEntry(string.Format(text, args), entryLevel, DateTime.Now);
            state.LogEntries.Enqueue(entry);
        }
    }
}