using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySubTotalSummarySettingGeneric
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class MediaHierarchySubTotalSummarySettingGeneric<TSubTotal, TSetting>
    {
        /// <summary>
        /// Gets or sets the sub totals.
        /// </summary>
        /// <value>The sub totals.</value>
        public ICollection<TSubTotal> SubTotals { get; set; } = default!;

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ICollection<TSetting> Settings { get; set; } = default!;
    }
}