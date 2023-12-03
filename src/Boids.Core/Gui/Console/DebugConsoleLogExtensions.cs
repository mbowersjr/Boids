using System;
using Microsoft.Extensions.Logging;
using Boids.Core.Gui.Views;

namespace Boids.Core.Gui.Console
{
    public static class DebugConsoleLogExtensions
    {
        public static DebugConsoleLogEntryLevel ToEntryLevel(this LogLevel logLevel)
        {
            DebugConsoleLogEntryLevel entryLevel = (DebugConsoleLogEntryLevel)(int)logLevel;
            return entryLevel;
        }

        public static void ToEntryLevel(this LogLevel logLevel, out DebugConsoleLogEntryLevel entryLevel)
        {
            entryLevel = (DebugConsoleLogEntryLevel)(int)logLevel;
        }

        public static LogLevel ToLogLevel(this DebugConsoleLogEntryLevel entryLevel)
        {
            LogLevel logLevel = (LogLevel)(int)entryLevel;
            return logLevel;
        }

        public static void ToLogLevel(this DebugConsoleLogEntryLevel entryLevel, out LogLevel logLevel)
        {
            logLevel = (LogLevel)(int)entryLevel;
        }
    }

    public static class DebugConsoleLogEntryExtensions
    {
        public static void LogTrace(this DebugConsoleState state, string text, params object[] args) => state.LogEntry(text, DebugConsoleLogEntryLevel.Trace, args);
        public static void LogDebug(this DebugConsoleState state, string text, params object[] args) => state.LogEntry(text, DebugConsoleLogEntryLevel.Debug, args);
        public static void LogInformation(this DebugConsoleState state, string text, params object[] args) => state.LogEntry(text, DebugConsoleLogEntryLevel.Information, args);
        public static void LogWarning(this DebugConsoleState state, string text, params object[] args) => state.LogEntry(text, DebugConsoleLogEntryLevel.Warning, args);
        public static void LogError(this DebugConsoleState state, string text, params object[] args) => 
            state.LogEntry(text, DebugConsoleLogEntryLevel.Error, args);
        public static void LogCritical(this DebugConsoleState state, string text, params object[] args) => state.LogEntry(text, DebugConsoleLogEntryLevel.Critical, args);

        public static void LogEntry(this DebugConsoleState state, string text, DebugConsoleLogEntryLevel entryLevel, params object[] args)
        {
            if (string.IsNullOrEmpty(text)) return;

            var entry = new DebugConsoleLogEntry(string.Format(text, args), entryLevel, DateTime.Now);
            state.LogEntries.Enqueue(entry);
        }
    }

}