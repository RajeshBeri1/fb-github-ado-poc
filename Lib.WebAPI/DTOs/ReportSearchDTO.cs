using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ReportSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReportSearchDTO : NamedSearch
    {
        /// <summary>
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }
    }
}