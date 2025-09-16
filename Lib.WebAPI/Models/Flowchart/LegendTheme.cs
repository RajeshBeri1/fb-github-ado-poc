using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// TotalsTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LegendTheme
    {
        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid? TableId { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the name of the Settings.
        /// </summary>
        /// <value>The name of the Settings.</value>
        public ICollection<LegendThemeSetting>? Settings { get; set; }

        /// <summary>
        /// Gets or sets the display verticle.
        /// </summary>
        /// <value>The display identifier.</value>
        public bool? DisplayVertically { get; set; }
    }
}
