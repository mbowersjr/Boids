using System;

namespace Boids.Core.Gui.Console
{
    public struct DebugConsoleLogEntry
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public DebugConsoleLogEntryLevel EntryLevel { get; set; }

        public DebugConsoleLogEntry(string text, DebugConsoleLogEntryLevel entryLevel)
            : this(text, entryLevel, DateTime.Now)
        {
        }

        public DebugConsoleLogEntry(string text, DebugConsoleLogEntryLevel entryLevel, DateTime timestamp)
        {
            Text = text;
            EntryLevel = entryLevel;
            Timestamp = timestamp;
        }
    }
}