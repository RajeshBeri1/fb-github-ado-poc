using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// SummaryTemplateSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryTemplateSearchDTO : NamedSearch
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }
    }
}