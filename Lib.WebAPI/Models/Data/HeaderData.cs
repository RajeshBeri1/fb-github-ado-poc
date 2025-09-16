using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// HeaderData
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderData
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public HeaderDataDetails? Details { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<HeaderDataRow> Rows { get; set; } = default!;
    }
}