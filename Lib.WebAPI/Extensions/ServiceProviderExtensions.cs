using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.WebAPI.Extensions
{
    /// <summary>
    /// IServiceProviderExtensions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Resolves the exception handler.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static ExceptionHandler ResolveExceptionHandler(this IServiceProvider serviceProvider)
        {
            var handler = serviceProvider.GetService<ExceptionHandler>();

            if (handler == null)
            {
                throw new Exception($"{nameof(ExceptionHandler)} required.");
            }

            return handler;
        }
    }
}