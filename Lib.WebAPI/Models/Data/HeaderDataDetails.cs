using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// HeaderDataDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDataDetails
    {
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<HeaderDataRow> Rows { get; set; } = default!;
    }
}