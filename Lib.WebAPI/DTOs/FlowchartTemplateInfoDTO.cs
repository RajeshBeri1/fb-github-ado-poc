using Lib.WebAPI.Enums;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// TemplateInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateInfoDTO : NamedModelBaseDTO
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
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the bool.
        /// </summary>
        /// <value>The IsReportAvailable.</value>
        public bool IsReportAvailable { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the bool.
        /// </summary>
        /// <value>The IsRunConfigurationAvailable.</value>
        public bool IsRunConfigurationAvailable { get; set; } = false;

        /// <summary>
        /// Gets or sets the parentId.
        /// </summary>
        /// <value>The parentId.</value>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the share status.
        /// </summary>
        /// <value>The Share Status.</value>
        public TemplateShareStatus ShareStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}