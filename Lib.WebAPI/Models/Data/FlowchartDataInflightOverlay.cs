using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// FlowchartDataInflightOverlay
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDataInflightOverlay
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the metrics.
        /// </summary>
        /// <value>The metrics.</value>
        public ICollection<FlowchartDataMetric> Metrics { get; set; } = default!;

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
    }
}