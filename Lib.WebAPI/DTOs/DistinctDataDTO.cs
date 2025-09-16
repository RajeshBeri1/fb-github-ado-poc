using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DistinctDataDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DistinctDataDTO
    {
        /// <summary>
        /// Gets or sets the name of the ParentColumnName.
        /// </summary>
        /// <value>The name of the ParentColumnName.</value>
        public string? ParentColumnName { get; set; }

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid? ParentTableId { get; set; }

        /// <summary>
        /// Gets or sets the name of the ParentSelectedValue.
        /// </summary>
        /// <value>The name of the ParentSelectedValue.</value>
        public string? ParentSelectedValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the table identifier.
        /// </summary>
        /// <value>The table identifier.</value>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the Levels for filtering values.
        /// </summary>
        /// <value>The Levels.</value>
        public IEnumerable<LevelDataDTO>? Levels { get; set; } = null;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime? EndDate { get; set; }
    }
}