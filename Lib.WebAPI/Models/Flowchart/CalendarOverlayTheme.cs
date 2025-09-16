using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarOverlayTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTheme
    {
        /// <summary>
        /// Gets or sets the section styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling SectionStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the row styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling RowStyling { get; set; } = default!;
    }
}