using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// UpdatePlannedMediaMigrationDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpdatePlannedMediaMigrationDTO : CreatePlannedMediaMigrationDTO
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the Removed
        /// </summary>
        public bool Removed { get; set; }
    }
}
