using Lib.WebAPI.Models.Flowchart;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// MediaHierarchyColumnAndTableDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyColumnAndTableDetails
    {
        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        /// <value>The column name.</value>
        public string? ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid? TableId { get; set; }
    }
}
