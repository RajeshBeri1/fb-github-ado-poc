using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// ThemeDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeDefinition
    {
        /// <summary>
        /// Gets or sets the calendar overlay theme.
        /// </summary>
        /// <value>The calendar overlay theme.</value>
        public CalendarOverlayTheme CalendarOverlayTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the calendar theme.
        /// </summary>
        /// <value>The calendar theme.</value>
        public CalendarTheme CalendarTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the footer theme.
        /// </summary>
        /// <value>The footer theme.</value>
        public FooterTheme FooterTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the grand total theme.
        /// </summary>
        /// <value>The grand total theme.</value>
        public GrandTotalTheme GrandTotalTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the header theme.
        /// </summary>
        /// <value>The header theme.</value>
        public HeaderTheme HeaderTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the media hierarchy theme.
        /// </summary>
        /// <value>The media hierarchy theme.</value>
        public MediaHierarchyTheme MediaHierarchyTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the media hierarchy level with style setting.
        /// </summary>
        /// <value>The media hierarchy level with style setting.</value>
        public List<MediaHierarchyLevelsWithStyles>? MediaHierarchyLevelsWithStyles { get; set; }

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the totals theme.
        /// </summary>
        /// <value>The totals theme.</value>
        public TotalsTheme TotalsTheme { get; set; } = default!;

        /// <summary>
        /// Gets or sets the legend theme.
        /// </summary>
        /// <value>The legend theme.</value>
        public LegendTheme? LegendTheme { get; set; }

        /// <summary>
        /// Gets or sets the Split By Column.
        /// </summary>
        /// <value>The Split By Column.</value>
        public SplitByColumn? SplitByColumn { get; set; }
    }
}