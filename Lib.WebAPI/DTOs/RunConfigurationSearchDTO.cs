using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// RunConfigurationSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationSearchDTO : NamedSearch
    {
        /// <summary>
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }
    }
}