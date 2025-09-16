using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartTemplateCreateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateCreateDTO
    {
        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public FlowchartDefinition? Definition { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string TemplateName { get; set; } = default!;
    }
}