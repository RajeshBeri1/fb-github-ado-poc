using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartTemplateShareInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateShareInfoDTO
    {
        /// <summary>
        /// Gets or sets the component details.
        /// </summary>
        /// <value>
        /// The component details.
        /// </value>
        public List<ComponentDetails> ComponentDetailsList { get; set; }

        /// <summary>
        /// Gets or sets the client details.
        /// </summary>
        /// <value>
        /// The client details.
        /// </value>
        public List<ClientDetails> ClientDetailsList { get; set; }

        /// <summary>
        /// Gets or sets the TemplateName.
        /// </summary>
        /// <value>
        /// The TemplateName.
        /// </value>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the TemplateId.
        /// </summary>
        /// <value>
        /// The TemplateId.
        /// </value>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the parentId.
        /// </summary>
        /// <value>The parentId.</value>
        public Guid? ParentId { get; set; }
    }

    public class ComponentDetails
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>
        /// The Name.
        /// </value>
        public string ComponentName { get; set; }

        /// <summary>
        /// Gets or sets the ComponentTemplateId.
        /// </summary>
        /// <value>
        /// The ComponentTemplateId.
        /// </value>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>
        /// The Name.
        /// </value>
        public string Name { get; set; }
    }

    public class ClientDetails
    {
        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>
        /// The ClientId.
        /// </value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the ClientName.
        /// </summary>
        /// <value>
        /// The ClientName.
        /// </value>
        public string Name { get; set; }
    }
}