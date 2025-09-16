using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// SplitByColumn
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SplitByColumn
    {
        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid? TableId { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string? ColumnName { get; set; }
    }
}
