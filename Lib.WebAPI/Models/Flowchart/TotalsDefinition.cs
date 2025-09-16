using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// TotalsDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsDefinition
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ICollection<TotalsColumn> Columns { get; set; } = default!;
    }
}