using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarOverlaySection
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlaySection
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<CalendarOverlayRow> Rows { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; } = default!;
    }
}