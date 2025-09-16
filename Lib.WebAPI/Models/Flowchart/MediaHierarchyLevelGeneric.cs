using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyLevelGeneric
    /// </summary>
    /// <typeparam name="TSetting">The type of the setting.</typeparam>
    /// <typeparam name="TSubTotal">The type of the sub total.</typeparam>
    /// <typeparam name="TInflightOverlay">The type of the inflight overlay.</typeparam>
    /// <typeparam name="TSubTotalSetting">The type of the sub total setting.</typeparam>

    [ExcludeFromCodeCoverage]
    public abstract class MediaHierarchyLevelGeneric<TSetting, TSubTotal, TInflightOverlay, TSubTotalSetting>
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ICollection<TSetting> Settings { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sub totals.
        /// </summary>
        /// <value>The sub totals.</value>
        public ICollection<TSubTotal>? SubTotals { get; set; }

        /// <summary>
        /// Gets or sets the inflight overlays.
        /// </summary>
        /// <value>The inflight overlays.</value>
        public ICollection<TInflightOverlay>? InflightOverlays { get; set; }

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the sub totals settings.
        /// </summary>
        /// <value>The sub total settings.</value>
        public ICollection<TSubTotalSetting>? SubTotalSettings { get; set; }
    }
}