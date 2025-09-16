using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// CalendarTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarTemplateDetailsDTO : CalendarTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public CalendarDefinition Definition { get; set; } = default!;
    }
}