using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartTemplateSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateSearchDTO : NamedSearch
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the removed.
        /// </summary>
        /// <value>The removed.</value>
        public bool Removed { get; set; }
    }
}