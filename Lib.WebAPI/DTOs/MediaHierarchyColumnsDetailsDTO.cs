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
    /// MediaHierarchyColumnsDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyColumnsDetailsDTO
    {
        /// <summary>
        /// Gets or sets the OmniClientId identifier.
        /// </summary>
        /// <value>The OmniClientId identifier.</value>
        public Guid? OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the ClientName.
        /// </summary>
        /// <value>The ClientName.</value>
        public string? ClientName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>The ClientId.</value>
        public string? ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public List<MediaHierarchyColumnAndTableDetails>? ColumnInfos { get; set; } = default!;
    }
}
