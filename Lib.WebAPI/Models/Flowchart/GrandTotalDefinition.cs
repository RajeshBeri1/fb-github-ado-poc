using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// GrandTotalDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalDefinition
    {
        /// <summary>
        /// Gets or sets the selections.
        /// </summary>
        /// <value>The selections.</value>
        public ICollection<GrandTotalSelection> Selections { get; set; } = default!;
    }
}