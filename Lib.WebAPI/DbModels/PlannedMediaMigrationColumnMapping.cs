using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// PlannedMediaMigrationColumnMapping
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PlannedMediaMigrationColumnMapping : DbModelBase
    {
        /// <summary>
        /// Gets or sets the sourceColumnName identifier.
        /// </summary>
        /// <value>The sourceColumnName identifier.</value>
        public string SourceColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the sourceTableId identifier.
        /// </summary>
        /// <value>The sourceTableId identifier.</value>
        public Guid SourceTableId { get; set; }

        /// <summary>
        /// Gets or sets the destinationColumnName identifier.
        /// </summary>
        /// <value>The destinationColumnName identifier.</value>
        public string DestinationColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the destinationTableId identifier.
        /// </summary>
        /// <value>The destinationTableId identifier.</value>
        public Guid DestinationTableId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid? OmniClientId { get; set; }
    }
}
