using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// HeaderRow
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderRow
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>The date format.</value>
        public HeaderDateFormat? DateFormat { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid? TableId { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public HeaderRowType Type { get; set; }

        /// <summary>
        /// Gets or sets the value separator.
        /// </summary>
        /// <value>The value separator.</value>
        public ValueSeparator? ValueSeparator { get; set; }
    }
}