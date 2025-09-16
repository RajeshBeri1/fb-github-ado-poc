using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DataDictionaryTableDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDictionaryTableDetailsDTO : NamedModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ICollection<DataDictionaryColumnDetailsDTO> Columns { get; set; } = default!;
    }
}