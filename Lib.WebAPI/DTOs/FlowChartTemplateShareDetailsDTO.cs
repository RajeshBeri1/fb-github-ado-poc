using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowChartTemplateShareDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowChartTemplateShareDetailsDTO
    {
        /// <summary>
        /// Gets or sets the CurrentOmniGuid.
        /// </summary>
        /// <value>
        /// The CurrentOmniGuid.
        /// </value>
        public Guid CurrentOmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the NewOmniClientId.
        /// </summary>
        /// <value>
        /// The NewOmniClientId.
        /// </value>
        public Guid NewOmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the NewTemplateName.
        /// </summary>
        /// <value>
        /// The NewTemplateName.
        /// </value>
        public string NewTemplateName { get; set; }

        /// <summary>
        /// Gets or sets the TemplateId.
        /// </summary>
        /// <value>
        /// The TemplateId.
        /// </value>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the ComponentInfosList.
        /// </summary>
        /// <value>
        /// The ComponentInfosList.
        /// </value>
        public List<ComponentInfo> ComponentInfosList { get; set; }
    }

    public class ComponentInfo
    {
        /// <summary>
        /// Gets or sets the TemplateId.
        /// </summary>
        /// <value>
        /// The TemplateId.
        /// </value>
        public Guid TemplateId { get; set; }
    }
}