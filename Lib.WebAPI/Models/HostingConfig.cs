using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// HostingConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HostingConfig
    {
        /// <summary>
        /// Gets the allowed origins
        /// </summary>
        /// <value>Allowed origins</value>
        public string AllowedOrigins { get; } = default!;
    }
}