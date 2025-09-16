using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySubTotal
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotal : MediaHierarchySubTotalBase
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
    }
}