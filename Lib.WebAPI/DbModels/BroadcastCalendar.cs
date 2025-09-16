using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// BroadcastCalendar
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BroadcastCalendar : DbModelBase
    {
        /// <summary>
        /// Gets or sets the broadcast calendar items.
        /// </summary>
        /// <value>The broadcast calendar items.</value>
        public string BroadcastCalendarItems { get; set; } = default!;

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>The year.</value>
        public long Year { get; set; }
    }
}