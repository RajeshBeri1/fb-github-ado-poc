using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartTemplateVersionSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateVersionSearchDTO
    {
        /// <summary>
        /// Gets or sets FlowchartTemplateId
        /// </summary>
        public Guid FlowchartTemplateId { get; set; }

        /// <summary>
        /// Gets or sets version
        /// </summary>
        public int Version { get; set; }
    }
}
