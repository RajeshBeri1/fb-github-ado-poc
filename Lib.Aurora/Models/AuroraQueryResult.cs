using Npgsql.Schema;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
namespace Lib.Aurora.Models
{
    /// <summary>
    /// AuroraQueryResult
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AuroraQueryResult
    {
        /// <summary>
        /// Gets or sets the column information.
        /// </summary>
        /// <value>The column information.</value>
        public ICollection<NpgsqlDbColumn> ColumnInfo { get; set; } = default!;

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ICollection<Dictionary<string,object?>> Rows { get; set; } = new Collection<Dictionary<string, object?>>();
    }
}