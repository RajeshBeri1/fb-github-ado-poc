using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// TotalsTemplate
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsTemplate : NamedDbModelBase
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
        public Guid? OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the totals definition.
        /// </summary>
        /// <value>The totals definition.</value>
        public string TotalsDefinition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}