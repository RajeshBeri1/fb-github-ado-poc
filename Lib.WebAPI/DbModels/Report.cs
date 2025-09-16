using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Enumerations;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// Report
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Report : NamedDbModelBase
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
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>The type of the file.</value>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the flowchart template.
        /// </summary>
        /// <value>The flowchart template.</value>
        public virtual FlowchartTemplate FlowchartTemplate { get; set; } = default!;

        /// <summary>
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the flowchart template version.
        /// </summary>
        /// <value>The flowchart template version.</value>
        public int FlowchartTemplateVersion { get; set; }

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
        /// Gets or sets the run configuration.
        /// </summary>
        /// <value>The run configuration.</value>
        public virtual RunConfiguration RunConfiguration { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run configuration identifier.
        /// </summary>
        /// <value>The run configuration identifier.</value>
        public Guid RunConfigurationId { get; set; }

        /// <summary>
        /// Gets or sets the report comments.
        /// </summary>
        /// <value>The report comments.</value>
        [MaxLength(100)]
        public string? Comments { get; set; }
    }
}