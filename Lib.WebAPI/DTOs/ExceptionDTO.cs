using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ExceptionDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExceptionDTO
    {
        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>The inner exception.</value>
        public ExceptionDTO? InnerException { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; } = default!;

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string?[] StackTrace { get; set; } = default!;
    }
}