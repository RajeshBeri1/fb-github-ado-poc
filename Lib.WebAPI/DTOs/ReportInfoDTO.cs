using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ReportInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReportInfoDTO : NamedModelBaseDTO
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
        /// Gets or sets the flowchart template version.
        /// </summary>
        /// <value>The flowchart template version.</value>
        public int FlowchartTemplateVersion { get; set; }

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
        /// Gets or sets the run configuration.
        /// </summary>
        /// <value>The run configuration.</value>
        public RunConfigurationInfoDTO RunConfiguration { get; set; } = default!;

        /// <summary>
        /// Gets or sets the report comments.
        /// </summary>
        /// <value>The report comments.</value>
        public string? Comments { get; set; }
    }
}