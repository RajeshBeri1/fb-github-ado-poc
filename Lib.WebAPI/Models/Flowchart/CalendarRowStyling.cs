using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarRowStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarRowStyling
    {
        /// <summary>
        /// Gets or sets the color of the alternate background.
        /// </summary>
        /// <value>The color of the alternate background.</value>
        public Color? AlternateBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor { get; set; } = default!;
    }
}