using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// TotalsTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsTheme
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the header styling.
        /// </summary>
        /// <value>The header styling.</value>
        public Styling HeaderStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sum styling.
        /// </summary>
        /// <value>The sum styling.</value>
        public Styling SumStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the main header styling.
        /// </summary>
        /// <value>The header styling.</value>
        public Styling? MainHeaderStyling { get; set; } = null;
    }
}