using System.Diagnostics.CodeAnalysis;
using Lib.Common.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// CorrelationIdMiddleware
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CorrelationIdMiddleware
    {
        /// <summary>
        /// The correlation identifier header
        /// </summary>
        public const string CorrelationIdHeader = "X-Correlation-Id";

        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationIdMiddleware" />
        /// class.
        /// </summary>
        /// <param name="next">The next.</param>
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="log">The log.</param>
        public async Task Invoke(HttpContext context, ICorrelationIdGenerator generator, ILog<CorrelationIdMiddleware> log)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = generator.CorrelationId;
                log.Add(LogLevel.Information, $"Using generated correlation id: {correlationId}");
            }
            else
            {
                generator.CorrelationId = correlationId;
                log.Add(LogLevel.Information, $"Using provided correlation id: {correlationId}");
            }

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(CorrelationIdHeader, correlationId);
                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}