using Microsoft.Extensions.Logging;

namespace Boids.Core.Gui
{
    public static class ConsoleLogEntryLevelExtensions
    {
        public static ConsoleLogEntryLevel ToEntryLevel(this LogLevel logLevel)
        {
            ConsoleLogEntryLevel entryLevel = (ConsoleLogEntryLevel)((int)logLevel);
            return entryLevel;
        }
        
        public static void ToEntryLevel(this LogLevel logLevel, out ConsoleLogEntryLevel entryLevel)
        {
            entryLevel = (ConsoleLogEntryLevel)((int)logLevel);
        }
        
        public static LogLevel ToLogLevel(this ConsoleLogEntryLevel entryLevel)
        {
            LogLevel logLevel = (LogLevel)((int)entryLevel);
            return logLevel;
        }
        
        public static void ToLogLevel(this ConsoleLogEntryLevel entryLevel, out LogLevel logLevel)
        {
            logLevel = (LogLevel)((int)entryLevel);
        }
    }
}