using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// CreatePlannedMediaMigrationDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreatePlannedMediaMigrationDTO
    {
        /// <summary>
        /// Gets or sets the SourceColumnName.
        /// </summary>
        /// <value>The SourceColumnName.</value>
        public string SourceColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the SourceTableId.
        /// </summary>
        /// <value>The SourceTableId.</value>
        public Guid SourceTableId { get; set; }

        /// <summary>
        /// Gets or sets the DestinationColumnName.
        /// </summary>
        /// <value>The DestinationColumnName.</value>
        public string DestinationColumnName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the DestinationTableId.
        /// </summary>
        /// <value>The DestinationTableId.</value>
        public Guid DestinationTableId { get; set; }
    }
}
