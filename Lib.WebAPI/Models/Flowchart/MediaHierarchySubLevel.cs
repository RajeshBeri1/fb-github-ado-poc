using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySubLevel
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubLevel : MediaHierarchySubLevelGeneric<MediaHierarchySubLevelSetting, MediaHierarchySubTotal>
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>
        /// The styling.
        /// </value>
        public Styling Styling { get; set; } = default!;
    }
}