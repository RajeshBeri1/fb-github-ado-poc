using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// ClientCalendar
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientCalendar : DbModelBase
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public string ClientCalendarItems { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client year.
        /// </summary>
        /// <value>The client year.</value>
        public string ClientYear { get; set; } = default!;
    }
}