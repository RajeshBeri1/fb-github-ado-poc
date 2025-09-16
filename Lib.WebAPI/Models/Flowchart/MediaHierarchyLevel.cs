using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyLevel
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyLevel : MediaHierarchyLevelGeneric<MediaHierarchySetting, MediaHierarchySubTotal, MediaHierarchyInflightOverlay, MediaHierarchySubTotalSummary>
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}