using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarOverlayDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayDefinition
    {
        /// <summary>
        /// Gets or sets the overlay sections.
        /// </summary>
        /// <value>The overlay sections.</value>
        public ICollection<CalendarOverlaySection> OverlaySections { get; set; } = default!;
    }
}