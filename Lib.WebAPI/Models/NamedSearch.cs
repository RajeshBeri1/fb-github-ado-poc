using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// NamedSearch
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NamedSearch
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [order ascending].
        /// </summary>
        /// <value><c>true</c> if [order ascending]; otherwise, <c>false</c>.</value>
        public bool OrderAscending { get; set; }

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>The search text.</value>
        public string? SearchText { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public int Start { get; set; }
    }
}