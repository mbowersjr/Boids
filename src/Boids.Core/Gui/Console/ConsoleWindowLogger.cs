using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Num = System.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Boids.Core.Gui.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Boids.Core.Gui.Console
{
    public class ConsoleWindowLoggerOptions
    {
        public int EventId { get; set; }

        public List<ConsoleLogEntryLevel> LogLevels { get; set; } =
            new List<ConsoleLogEntryLevel>()
            {
                ConsoleLogEntryLevel.Information,
                ConsoleLogEntryLevel.Warning,
                ConsoleLogEntryLevel.Error
            };

        public IDebugConsoleWindow ConsoleWindow { get; set; }

        public ConsoleWindowLoggerOptions(IDebugConsoleWindow consoleWindow)
        {
            ConsoleWindow = consoleWindow;
        }
    }

    public class ConsoleWindowLogger : ILogger
    {
        private readonly string _name;
        private readonly ConsoleWindowLoggerOptions _config;
        private IDebugConsoleWindow _consoleWindow;

        public ConsoleWindowLogger(string name, ConsoleWindowLoggerOptions config)
        {
            _name = name;
            _config = config;
            _consoleWindow = _config.ConsoleWindow;
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

                _consoleWindow.ConsoleState.LogEntry(formatted, entryLevel);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _config.LogLevels.Contains(logLevel.ToEntryLevel());
        }

        public IDisposable BeginScope<TState>(TState state) => default;
    }

    public class ConsoleWindowLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsoleWindowLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string name)
        {
            var config = ActivatorUtilities.CreateInstance<ConsoleWindowLoggerOptions>(_serviceProvider);
            return new ConsoleWindowLogger(name, config);
        }

        public void Dispose()
        {
        }
    }
}