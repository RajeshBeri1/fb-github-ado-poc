using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// LegendThemeSetting
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LegendThemeSetting
    {
        /// <summary>
        /// Gets or sets the styling for flight bars.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LegendThemeSetting"
        /// /> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the setting inflight overlay styling.
        /// </summary>
        /// <value>The setting inflight overlay styling.</value>
        public Styling? InflightOverlayStyling { get; set; }

        /// <summary>
        /// Gets or sets the sub total styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling? SubTotalStyling { get; set; }
    }
}