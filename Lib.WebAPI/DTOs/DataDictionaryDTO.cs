using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DataDictionaryDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDictionaryDTO
    {
        /// <summary>
        /// Gets or sets the tables.
        /// </summary>
        /// <value>The tables.</value>
        public ICollection<DataDictionaryTableDetailsDTO> Tables { get; set; } = default!;
    }
}