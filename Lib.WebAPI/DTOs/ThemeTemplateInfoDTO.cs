using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ThemeTemplateInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeTemplateInfoDTO : NamedModelBaseDTO
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
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}