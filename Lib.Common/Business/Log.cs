using Lib.Common.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lib.Common.Business
{
    /// <summary>
    /// Log
    /// </summary>
    public class Log<T> : ILog<T>
    {
        private readonly ICorrelationIdGenerator generator;
        private readonly ILogger<T> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log{T}" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="generator">The generator.</param>
        public Log(ILogger<T> logger, ICorrelationIdGenerator generator)
        {
            this.logger = logger;
            this.generator = generator;
        }

        /// <summary>
        /// Adds the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Add(LogLevel logLevel, string message, Exception? exception = null)
        {
            if (!logger.IsEnabled(logLevel) || logLevel == LogLevel.None)
            {
                return;
            }

            var logMsg = "[{date:dd/MM/yyyy hh:mm:ss:fff}] - {{{correlationId}}} {message}";

            switch (logLevel)
            {
                case LogLevel.Critical:
                    logger.LogCritical(exception: exception, message: logMsg, DateTime.UtcNow, generator.CorrelationId, message);
                    return;

                case LogLevel.Debug:
                    logger.LogDebug(exception: exception, message: logMsg, DateTime.UtcNow, generator.CorrelationId, message);
                    break;

                case LogLevel.Error:
                    logger.LogError(exception: exception, message: logMsg, DateTime.UtcNow, generator.CorrelationId, message);
                    break;

                case LogLevel.Information:
                    logger.LogInformation(exception: exception, message: logMsg, DateTime.UtcNow, generator.CorrelationId, message);
                    break;

                case LogLevel.Trace:
                    logger.LogTrace(exception: exception, message: logMsg, DateTime.UtcNow, generator.CorrelationId, message);
                    break;

                case LogLevel.Warning:
                    logger.LogWarning(exception: exception, message: logMsg, DateTime.UtcNow, generator.CorrelationId, message);
                    break;

                default:
                    throw new NotSupportedException($"Log level {logLevel} is not supported.");
            }
        }
    }
}