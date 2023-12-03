using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Num = System.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Boids.Core.Gui.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Boids.Core.Gui.Console
{
    public class DebugConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly DebugConsoleLoggerOptions _config;
        private DebugConsoleState _consoleState;

        public DebugConsoleLogger(string name, DebugConsoleLoggerOptions config, IServiceProvider services)
        {
            _name = name;
            _config = config;

            _consoleState = services.GetService<DebugConsoleState>();
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                var entryLevel = logLevel.ToEntryLevel();
                var formatted = formatter(state, exception);

                _consoleState.LogEntry(formatted, entryLevel);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _config.LogLevels.Contains(logLevel.ToEntryLevel());
        }

        public IDisposable BeginScope<TState>(TState state) => default;
    }
}