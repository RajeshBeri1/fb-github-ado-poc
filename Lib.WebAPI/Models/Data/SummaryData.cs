using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// SummaryData
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryData
    {
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<SummaryDataRow> Rows { get; set; } = default!;
    }
}