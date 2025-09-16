using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// CalendarItem
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarItem
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the json value.
        /// </summary>
        /// <value>The json value.</value>
        public string JsonValue { get; set; } = default!;
    }
}