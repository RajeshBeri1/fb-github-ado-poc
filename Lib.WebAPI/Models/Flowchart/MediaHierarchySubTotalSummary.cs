using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySubTotalSetting
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalSummary : MediaHierarchySubTotalSummarySettingGeneric<MediaHierarchySubTotalBase, MediaHierarchySubTotalSummarySettingBase>
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the title styling.
        /// </summary>
        /// <value>
        /// The title styling.
        /// </value>
        public Styling TitleStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }


        /// <summary>
        /// Gets or sets the DisplayNames.
        /// </summary>
        /// <value>The order.</value>
        public Dictionary<string, string>? DisplayNames { get; set; } = null;
    }
}