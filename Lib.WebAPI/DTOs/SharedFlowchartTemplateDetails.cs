using Lib.WebAPI.DbModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// SharedFlowchartTemplateDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SharedFlowchartTemplateDetails : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the Omni Client Name.
        /// </summary>
        /// <value>The Omni Client Name.</value>
        public string OmniClientName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Omni Client Name.
        /// </summary>
        /// <value>The Omni Client Name.</value>
        public string CreatedByUser { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Omni Client Name.
        /// </summary>
        /// <value>The Omni Client Name.</value>
        public string ClientName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the created by user identifier.
        /// </summary>
        /// <value>The created by user identifier.</value>
        public Guid CreatedByUserId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the modified by user identifier.
        /// </summary>
        /// <value>The modified by user identifier.</value>
        public Guid ModifiedByUserId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>The modified date.</value>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets ClientMappingClientId.
        /// </summary>
        /// <value>The ClientMappingClientId.</value>
        public Guid ClientMappingClientId { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; } = 1;
    }
}
