using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// SummaryTemplate
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryTemplate : NamedDbModelBase
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
        /// Gets or sets the summary definition.
        /// </summary>
        /// <value>The summary definition.</value>
        public string SummaryDefinition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public int Version { get; set; } = 1;
    }
}