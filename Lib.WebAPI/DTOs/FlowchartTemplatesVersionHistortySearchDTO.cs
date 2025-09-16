using Lib.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartTemplatesVersionHistortySearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplatesVersionHistortySearchDTO : NamedSearch
    {
        /// <summary>
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }
    }
}