using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA
{
    public static class Logging
    {
        private static readonly ILoggerFactory LoggerFactory =
            Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder
#if POLYQA_VERBOSE
                    .SetMinimumLevel(LogLevel.Debug)
#else
                    .SetMinimumLevel(LogLevel.Information)
#endif
                    .AddProvider(new UnityLoggerProvider());
            });

        public static ILogger<T> CreateLogger<T>() => new Logger<T>(LoggerFactory);

        private class UnityLoggerProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName) => new UnityLogger();
            public void Dispose() { }
        }

        private class UnityLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) => null;
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                var message = exception == null ?
                    $"[PolyQA] {formatter(state, null)}" :
                    $"[PolyQA] {formatter(state, exception)}\n{NormalizeMessage(exception.Message)}\n{exception.StackTrace}";
                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        Debug.Log(message);
                        break;
                    case LogLevel.Warning:
                    case LogLevel.Error:
                        Debug.LogWarning(message);
                        break;
                    case LogLevel.Critical:
                        Debug.LogError(message);
                        break;
                    case LogLevel.None:
                        break;
                }
            }

            private string NormalizeMessage(string message)
            {
                // C#の日本語のログが改行以降文字化けして、スタックトレースを消してしまうので削除
                var index = message.IndexOf("\r\n\0", StringComparison.Ordinal);
                return index >= 0 ? message.Substring(0, index) : message;
            }
        }
    }
}