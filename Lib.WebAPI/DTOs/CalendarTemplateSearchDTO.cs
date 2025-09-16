using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// CalendarTemplateSearchListDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarTemplateSearchDTO : NamedSearch
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