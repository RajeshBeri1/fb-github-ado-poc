using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySubLevelSettingGeneric
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class MediaHierarchySubLevelSettingGeneric<TSubTotal>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MediaHierarchySetting"
        /// /> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the flight range.
        /// </summary>
        /// <value>The flight range.</value>
        public FlightRange FlightRange { get; set; }

        /// <summary>
        /// Gets or sets the name of the inflight overlay column.
        /// </summary>
        /// <value>The name of the inflight overlay column.</value>
        public string? InflightOverlayColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the inflight overlay table identifier.
        /// </summary>
        /// <value>The inflight overlay table identifier.</value>
        public Guid? InflightOverlayTableId { get; set; }

        /// <summary>
        /// Gets or sets the name of the metric column.
        /// </summary>
        /// <value>The name of the metric column.</value>
        public string? MetricColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the metric table identifier.
        /// </summary>
        /// <value>The metric table identifier.</value>
        public Guid? MetricTableId { get; set; }

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
        /// Gets or sets the sub totals.
        /// </summary>
        /// <value>The sub totals.</value>
        public ICollection<TSubTotal>? SubTotals { get; set; }
    }
}