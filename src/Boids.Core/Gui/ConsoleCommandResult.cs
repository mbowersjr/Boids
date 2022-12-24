using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Extensions.Logging;

namespace Boids.Core.Gui
{
    public class ConsoleCommandResult
    {
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public bool WasError { get; set; }

        public ConsoleCommandResult(string inputText, string outputText, bool wasError)
        {
            InputText = inputText;
            OutputText = outputText;
            WasError = wasError;
        }

        public static ConsoleCommandResult Create(string inputText, string outputText, bool wasError = false) => 
            new ConsoleCommandResult(inputText, outputText, wasError);

        public static readonly ConsoleCommandResult EmptyResult = new ConsoleCommandResult(null, null, false);
    }
}
