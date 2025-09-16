using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// FusionCacheConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FusionCacheConfig
    {
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FusionCacheConfig" />
        /// is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public CacheItemPriority Priority { get; set; }
    }
}