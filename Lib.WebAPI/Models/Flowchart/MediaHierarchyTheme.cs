using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyTheme
    {
        /// <summary>
        /// Gets or sets the flight bar styling.
        /// </summary>
        /// <value>The flight bar styling.</value>
        public Styling FlightBarStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the setting inflight overlay styling.
        /// </summary>
        /// <value>The setting inflight overlay styling.</value>
        public Styling InflightOverlayStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the left menu styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling LeftMenuStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sub total styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling SubTotalStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sub total title styling.
        /// </summary>
        /// <value>The sub total title styling.</value>
        public Styling SubTotalTitleStyling { get; set; } = default!;
    }
}