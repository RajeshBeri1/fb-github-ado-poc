using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ListDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ListDTO<T> where T : ModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<T> Items { get; set; } = default!;

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; set; }
    }
}