using Microsoft.Extensions.Logging;

namespace Lib.Common.Business.Interfaces
{
    /// <summary>
    /// ILog
    /// </summary>
    public interface ILog<T>
    {
        /// <summary>
        /// Adds the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Add(LogLevel logLevel, string message, Exception? exception = null);
    }
}