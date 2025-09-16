using System.Diagnostics.CodeAnalysis;
using Amazon.Athena.Model;

namespace Lib.Athena.Models
{
    /// <summary>
    /// AthenaQueryResult
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AthenaQueryResult
    {
        /// <summary>
        /// Gets or sets the column information.
        /// </summary>
        /// <value>The column information.</value>
        public List<ColumnInfo> ColumnInfo { get; set; } = default!;

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public List<Row> Rows { get; set; } = new List<Row>();
    }
}