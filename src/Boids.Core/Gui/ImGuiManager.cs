using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Extensions.Logging;

namespace Boids.Core.Gui
{
    public class ConsoleCommandResult
    {
        public string Text { get; set; }
        public bool WasError { get; set; }

        public ConsoleCommandResult(string text, bool wasError)
        {
            Text = text;
            WasError = wasError;
        }

        public static ConsoleCommandResult Create(string text, bool wasError = false) => new ConsoleCommandResult(text, wasError);

        public static readonly ConsoleCommandResult EmptyResult = new ConsoleCommandResult(null, false);
    }
}
