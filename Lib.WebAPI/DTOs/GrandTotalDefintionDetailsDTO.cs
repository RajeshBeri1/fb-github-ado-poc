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
    /// GrandTotalDefintionDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalDefintionDetailsDTO
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>The Id.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public GrandTotalDefinition Definition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the CreatedByUserId.
        /// </summary>
        /// <value>The CreatedByUserId.</value>
        public Guid CreatedByUserId { get; set; }
    }
}
