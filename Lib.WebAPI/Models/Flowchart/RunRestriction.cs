using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// RunRestriction
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunRestriction
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the value json.
        /// </summary>
        /// <value>The value json.</value>
        public string ValueJson { get; set; } = default!;
    }
}