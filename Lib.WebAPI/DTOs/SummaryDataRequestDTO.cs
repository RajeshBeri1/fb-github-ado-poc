using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// SummaryDataRequestDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryDataRequestDTO
    {
        /// <summary>
        /// Gets or sets the calendar configuration.
        /// </summary>
        /// <value>The calendar configuration.</value>
        public CalendarConfiguration CalendarConfiguration { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public ICollection<RunRestriction> RunRestrictions { get; set; } = default!;

        /// <summary>
        /// Gets or sets the summary definition.
        /// </summary>
        /// <value>The summary definition.</value>
        public SummaryDefinition SummaryDefinition { get; set; } = default!;
    }
}