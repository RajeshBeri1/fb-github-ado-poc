using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DataDictionaryColumnDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDictionaryColumnDetailsDTO : DataDictionaryColumn
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; } = default!;
    }
}