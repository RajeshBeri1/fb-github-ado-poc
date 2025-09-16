using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// SummaryTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryTemplateDetailsDTO : SummaryTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public SummaryDefinition Definition { get; set; } = default!;
    }
}