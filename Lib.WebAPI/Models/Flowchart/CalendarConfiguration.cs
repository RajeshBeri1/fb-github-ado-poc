using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// CalendarConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarConfiguration
    {
        /// <summary>
        /// Gets or sets the custom end date.
        /// </summary>
        /// <value>The custom end date.</value>
        public DateTime? CustomEndDate { get; set; }

        /// <summary>
        /// Gets or sets the custom start date.
        /// </summary>
        /// <value>The custom start date.</value>
        public DateTime? CustomStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reporting time frame.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is reporting time frame; otherwise, <c>false</c>.
        /// </value>
        public bool IsReportingTimeFrame { get; set; }

        /// <summary>
        /// Gets or sets the reporting time frame.
        /// </summary>
        /// <value>The reporting time frame.</value>
        public CalendarReportingTimeFrame? ReportingTimeFrame { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<CalendarRow> Rows { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public CalendarType Type { get; set; }

        /// <summary>
        /// Gets or sets the normalized calendar.
        /// </summary>
        /// <value>The normalized calendar.</value>
        public bool? IsNormalizedEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the UseCustomStartDay for calendar.
        /// </summary>
        /// <value>The UseCustomStartDay for calendar.</value>
        public bool? UseCustomStartDay { get; set; } = false;
    }
}