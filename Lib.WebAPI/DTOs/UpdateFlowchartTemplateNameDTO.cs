using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// UpdateFlowchartTemplateNameDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpdateFlowchartTemplateNameDto
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the Flowchart template Id.
        /// </summary>
        /// <value>The Id of the flowchart template.</value>
        public Guid FlowchartTemplateId { get; set; } = Guid.Empty!;

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string TemplateName { get; set; } = default!;
    }
}
