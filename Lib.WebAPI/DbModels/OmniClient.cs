using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// OmniClient
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OmniClient : DbModelBase
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        public virtual Client Client { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; } = default!;

        /// <summary>
        /// Gets or sets the parent omni client identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        public Guid? ParentId { get; set; }
    }
}