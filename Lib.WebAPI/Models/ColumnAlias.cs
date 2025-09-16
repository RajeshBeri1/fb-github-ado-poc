using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// ColumnAlias
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ColumnAlias
    {
        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; } = default!;
    }
}