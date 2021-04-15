using System;

namespace Boids.Core.Gui
{
    public struct ConsoleLogEntry
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public ConsoleLogEntryLevel EntryLevel { get; set; }
        
        public ConsoleLogEntry(string text, ConsoleLogEntryLevel entryLevel)
            : this(text, entryLevel, DateTime.Now)
        {
        }

        public ConsoleLogEntry(string text, ConsoleLogEntryLevel entryLevel, DateTime timestamp)
        {
            Text = text;
            EntryLevel = entryLevel;
            Timestamp = timestamp;
        }
    }
}