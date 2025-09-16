using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// RunConfigurationInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationInfoDTO : NamedModelBaseDTO
    {
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
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }

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
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public string RunRestrictions { get; set; } = default!;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the run configuration comments.
        /// </summary>
        /// <value>The run configuration comments.</value>
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets the Custom Style Settings.
        /// </summary>
        /// <value>The Custom Style Settings.</value>
        public string? CustomStyleSettings { get; set; }

        /// <summary>
        /// Gets or sets the Track Changes.
        /// </summary>
        /// <value>The Track Format Changes.</value>
        public bool? TrackFormatChanges { get; set; }
    }
}