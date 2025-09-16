using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// ExceptionHandlerMiddleware
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware" />
        /// class.
        /// </summary>
        /// <param name="next">The next.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the asynchronous.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        public async Task InvokeAsync(HttpContext httpContext, ExceptionHandler exceptionHandler)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await exceptionHandler.HandleAsync(ex, httpContext);
            }
        }
    }
}