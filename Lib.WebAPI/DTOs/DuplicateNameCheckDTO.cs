using Lib.WebAPI.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DuplicateNameCheckDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DuplicateNameCheckDto
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the component.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the component.</value>
        public FlowChartComponent? ComponentName { get; set; } = default!;
    }
}
