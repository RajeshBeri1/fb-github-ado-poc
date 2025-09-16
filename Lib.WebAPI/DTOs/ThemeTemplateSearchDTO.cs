using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ThemeTemplateSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeTemplateSearchDTO : NamedSearch
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the Removed.
        /// </summary>
        /// <value>The Removed.</value>
        [DefaultValue(false)]
        public bool? Removed { get; set; }
    }
}