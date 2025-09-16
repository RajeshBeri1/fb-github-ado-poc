using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FlowchartTemplatesVersionHistortiesDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplatesVersionHistortiesDTO : NamedModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the flowchart template version.
        /// </summary>
        /// <value>The flowchart template version.</value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public string? FlowchartDefinition { get; set; }

        /// <summary>
        /// Gets or sets the created by user.
        /// </summary>
        /// <value>The created by user.</value>
        public UserInfoDTO CreatedByUser { get; set; } = default!;

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified by user.
        /// </summary>
        /// <value>The modified by user.</value>
        public UserInfoDTO ModifiedByUser { get; set; } = default!;

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>The modified date.</value>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ModelBaseDTO" /> is
        /// removed.
        /// </summary>
        /// <value><c>true</c> if removed; otherwise, <c>false</c>.</value>
        public bool Removed { get; set; }

        /// <summary>
        /// Gets or sets the flowchartTemplatesVersionHistorties comments.
        /// </summary>
        /// <value>The flowchartTemplatesVersionHistorties comments.</value>
        public string? Comments { get; set; }
    }
}
