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
    /// FlowchartDefinitionDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDefinitionDetailsDTO
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>The Id.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public FlowchartDefinition? Definition { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByUserId.
        /// </summary>
        /// <value>The CreatedByUserId.</value>
        public Guid CreatedByUserId { get; set; }
    }
}
