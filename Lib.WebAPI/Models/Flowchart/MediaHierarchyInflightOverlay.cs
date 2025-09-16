using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyInflightOverlay
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyInflightOverlay : MediaHierarchyInflightOverlayBase
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}