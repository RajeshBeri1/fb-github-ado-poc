using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyChannel
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySetting : MediaHierarchySettingGeneric<MediaHierarchySubLevel, MediaHierarchySubTotal, MediaHierarchyInflightOverlay,MediaHierarchySubTotalSummary>
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the inflight overlay styling.
        /// </summary>
        /// <value>
        /// The inflight overlay styling.
        /// </value>
        public Styling InflightOverlayStyling { get; set; } = default!;
    }
}