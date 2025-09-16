using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarOverlaySectionStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlaySectionStyling
    {
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor { get; set; } = default!;

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public CalendarOverlaySectionFormat Format { get; set; }
    }
}