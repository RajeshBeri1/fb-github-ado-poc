using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// TotalsColumn
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsColumn
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the header styling.
        /// </summary>
        /// <value>The header styling.</value>
        public Styling HeaderStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sum styling.
        /// </summary>
        /// <value>The sum styling.</value>
        public Styling SumStyling { get; set; } = default!;
    }
}