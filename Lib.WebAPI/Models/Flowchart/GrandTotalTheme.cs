using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// GrandTotalTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalTheme
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>
        /// The styling.
        /// </value>
        public Styling LeftMenuStyling { get; set; } = default!;
    }
}