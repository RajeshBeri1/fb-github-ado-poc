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
    /// MediaHierarchyDefinitionWithOmniClientId
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyDefinitionWithOmniClientId
    {
        /// <summary>
        /// Gets or sets the OmniClientId identifier.
        /// </summary>
        /// <value>The OmniClientId identifier.</value>
        public Guid? OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>The ClientId.</value>
        public string? ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string? ClientName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public MediaHierarchyDefinition? Definition { get; set; } = default!;
    }
}
