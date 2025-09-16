using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// GrandTotalData
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalData
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

        /// <summary>
        /// Gets or sets the local currency code for each metric level.
        /// </summary>
        /// <value>The local currency.</value>
        public string? LocalCurrency { get; set; }
    }
}