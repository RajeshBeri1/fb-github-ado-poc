using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// FlowchartDataLevel
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDataLevel
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the flight range.
        /// </summary>
        /// <value>The flight range.</value>
        public FlightRange FlightRange { get; set; }

        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        /// <value>The levels.</value>
        public ICollection<FlowchartDataLevel>? Levels { get; set; }

        /// <summary>
        /// Gets or sets the name of the metric column.
        /// </summary>
        /// <value>The name of the metric column.</value>
        public string MetricColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the metrics.
        /// </summary>
        /// <value>The metrics.</value>
        public ICollection<FlowchartDataMetric>? Metrics { get; set; }

        /// <summary>
        /// Gets or sets the metric table identifier.
        /// </summary>
        /// <value>The metric table identifier.</value>
        public Guid MetricTableId { get; set; }

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
        /// Gets or sets the sub levels.
        /// </summary>
        /// <value>The sub levels.</value>
        public ICollection<FlowchartDataSubLevel>? SubLevels { get; set; }

        /// <summary>
        /// Gets or sets the sub totals.
        /// </summary>
        /// <value>The sub totals.</value>
        public ICollection<FlowchartDataSubTotal>? SubTotals { get; set; }

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the totals.
        /// </summary>
        /// <value>The totals.</value>
        public ICollection<FlowchartDataTotal>? Totals { get; set; }

        /// <summary>
        /// Gets or sets the actual totals.
        /// </summary>
        /// <value>The totals.</value>
        public ICollection<FlowchartDataTotal>? ActualTotals { get; set; }

        /// <summary>
        /// Gets or sets the totals.
        /// </summary>
        /// <value>The BriefedCtcTotals.</value>
        public ICollection<FlowchartDataTotal>? BriefedCtcTotals { get; set; }

        /// <summary>
        /// Gets or sets the local currency code for each metric level.
        /// </summary>
        /// <value>The local currency.</value>
        public string? LocalCurrency { get; set; }


        /// <summary>
        /// Gets or sets the sub total summary.
        /// </summary>
        /// <value>The sub total summary.</value>
        public ICollection<FlowchartDataSubTotal>? SubTotalSummary { get; set; }
    }
}