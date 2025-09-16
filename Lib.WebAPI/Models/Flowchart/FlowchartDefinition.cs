using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// FlowchartDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDefinition
    {
        /// <summary>
        /// Gets or sets the calendar definition.
        /// </summary>
        /// <value>The calendar definition.</value>
        public ReferencedCalendarDefinition? CalendarDefinition { get; set; }

        /// <summary>
        /// Gets or sets the calendar overlay definition.
        /// </summary>
        /// <value>The calendar overlay definition.</value>
        public ReferencedCalendarOverlayDefinition? CalendarOverlayDefinition { get; set; }

        /// <summary>
        /// Gets or sets the footer definition
        /// </summary>
        /// <value>The footder definition.</value>
        public ReferencedFooterDefinition? FooterDefinition { get; set; }

        /// <summary>
        /// Gets or sets the grand total definition.
        /// </summary>
        /// <value>The grand total definition.</value>
        public ReferencedGrandTotalDefinition? GrandTotalDefinition { get; set; }

        /// <summary>
        /// Gets or sets the header definition.
        /// </summary>
        /// <value>The header definition.</value>
        public ReferencedHeaderDefinition? HeaderDefinition { get; set; }

        /// <summary>
        /// Gets or sets the media hierarchy definition.
        /// </summary>
        /// <value>The media hierarchy definition.</value>
        public ReferencedMediaHierarchyDefinition? MediaHierarchyDefinition { get; set; }

        /// <summary>
        /// Gets or sets the summary definition.
        /// </summary>
        /// <value>The summary definition.</value>
        public ReferencedSummaryDefinition? SummaryDefinition { get; set; }

        /// <summary>
        /// Gets or sets the theme definition.
        /// </summary>
        /// <value>The theme definition.</value>
        public ReferencedThemeDefinition? ThemeDefinition { get; set; }

        /// <summary>
        /// Gets or sets the totals definition.
        /// </summary>
        /// <value>The totals definition.</value>
        public ReferencedTotalsDefinition? TotalsDefinition { get; set; }
    }
}