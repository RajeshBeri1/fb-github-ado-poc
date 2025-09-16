using System.Diagnostics.CodeAnalysis;
using Lib.Athena.Enumerations;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// DataDictionaryColumn
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDictionaryColumn
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ColumnType Type { get; set; }
    }
}