using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// DataDictionaryTable
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDictionaryTable : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public string DataDictionaryColumns { get; set; } = default!;
    }
}