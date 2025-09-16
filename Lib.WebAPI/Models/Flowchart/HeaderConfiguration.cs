using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// HeaderConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderConfiguration
    {
        /// <summary>
        /// Gets or sets the column margin.
        /// </summary>
        /// <value>The column margin.</value>
        public int ColumnMargin { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public HeaderDetails? Details { get; set; }

        /// <summary>
        /// Gets or sets the logos.
        /// </summary>
        /// <value>The logos.</value>
        public ICollection<HeaderLogo>? Logos { get; set; }

        /// <summary>
        /// Gets or sets the row margin.
        /// </summary>
        /// <value>The row margin.</value>
        public int RowMargin { get; set; }

        /// <summary>
        /// Gets or sets the row merge.
        /// </summary>
        /// <value>The row merge.</value>
        public HeaderRowMerge RowMerge { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<HeaderRow> Rows { get; set; } = default!;
    }
}