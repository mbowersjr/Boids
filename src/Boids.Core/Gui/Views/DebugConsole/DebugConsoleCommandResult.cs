namespace Boids.Core.Gui.Views
{
    public class DebugConsoleCommandResult
    {
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public bool WasError { get; set; }

        public DebugConsoleCommandResult(string inputText, string outputText, bool wasError)
        {
            InputText = inputText;
            OutputText = outputText;
            WasError = wasError;
        }

        public static DebugConsoleCommandResult Create(string inputText, string outputText, bool wasError = false) =>
            new DebugConsoleCommandResult(inputText, outputText, wasError);

        public static readonly DebugConsoleCommandResult EmptyResult = new DebugConsoleCommandResult(null, null, false);
    }
}
