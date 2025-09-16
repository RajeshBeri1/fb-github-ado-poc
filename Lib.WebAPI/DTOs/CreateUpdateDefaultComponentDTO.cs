using Lib.WebAPI.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// CreateUpdateDefaultComponentDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateUpdateDefaultComponentDTO
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the TemplateId identifier.
        /// </summary>
        /// <value>The TemplateId identifier.</value>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the component.</value>
        public FlowChartComponent? ComponentName { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}
