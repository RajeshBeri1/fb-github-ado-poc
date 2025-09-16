using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// Template
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplate : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the created by user.
        /// </summary>
        /// <value>The created by user.</value>
        public virtual User CreatedByUser { get; set; } = default!;

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
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public string? FlowchartDefinition { get; set; }

        /// <summary>
        /// Gets or sets the modified by user.
        /// </summary>
        /// <value>The modified by user.</value>
        public virtual User ModifiedByUser { get; set; } = default!;

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
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public virtual ICollection<Report> Reports { get; set; } = new Collection<Report>();

        /// <summary>
        /// Gets or sets the run configurations.
        /// </summary>
        /// <value>The run configurations.</value>
        public virtual ICollection<RunConfiguration> RunConfigurations { get; set; } = new Collection<RunConfiguration>();

        /// <summary>
        /// Gets or sets the FlowchartTemplatesVersionHistorty.
        /// </summary>
        /// <value>The FlowchartTemplatesVersionHistorty.</value>
        public virtual ICollection<FlowchartTemplatesVersionHistorty> FlowchartTemplatesVersionHistorties { get; set; } = new Collection<FlowchartTemplatesVersionHistorty>();

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets the parentId.
        /// </summary>
        /// <value>The parentId.</value>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}