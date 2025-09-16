using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarTheme
    {
        /// <summary>
        /// Gets or sets the row styling.
        /// </summary>
        /// <value>The row styling.</value>
        public Styling RowStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}