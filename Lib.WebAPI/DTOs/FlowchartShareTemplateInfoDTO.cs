using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartShareTemplateInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartShareTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the TemplateId.
        /// </summary>
        /// <value>The TemplateId.</value>
        public Guid TemplateId { get; set; }

    }
}