using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarDefinition
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public CalendarConfiguration Configuration { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}