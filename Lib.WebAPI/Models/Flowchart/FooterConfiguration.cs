using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// FooterConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterConfiguration
    {
        /// <summary>
        /// Gets or sets the column margin.
        /// </summary>
        /// <value>The column margin.</value>
        public int ColumnMargin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable the legend
        /// visualization
        /// </summary>
        public bool EnableLegend { get; set; }

        /// <summary>
        /// Gets or sets the legend column count
        /// </summary>
        public int LegendColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the row margin.
        /// </summary>
        /// <value>The row margin.</value>
        public int RowMargin { get; set; }

        /// <summary>
        /// Gets or sets the row merge.
        /// </summary>
        /// <value>The row merge.</value>
        public FooterRowMerge RowMerge { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<FooterRow> Rows { get; set; } = default!;
    }
}