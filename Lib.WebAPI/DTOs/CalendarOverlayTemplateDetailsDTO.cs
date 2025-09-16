using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// CalendarOverlayTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTemplateDetailsDTO : CalendarOverlayTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public CalendarOverlayDefinition Definition { get; set; } = default!;
    }
}