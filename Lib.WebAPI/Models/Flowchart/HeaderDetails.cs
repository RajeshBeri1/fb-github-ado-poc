using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// HeaderDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDetails
    {
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<HeaderDetailsRow> Rows { get; set; } = default!;
    }
}