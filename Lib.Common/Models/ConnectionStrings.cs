using System.Diagnostics.CodeAnalysis;

namespace Lib.Common.Models
{
    /// <summary>
    /// ConnectionStrings
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConnectionStrings
    {
        /// <summary>
        /// Gets or sets the azurite BLOB.
        /// </summary>
        /// <value>The azurite BLOB.</value>
        public string AzuriteBlob { get; set; } = default!;

        /// <summary>
        /// Gets or sets the portal database.
        /// </summary>
        /// <value>The portal database.</value>
        public string PortalDb { get; set; } = default!;

        /// <summary>
        /// Gets or sets the redis.
        /// </summary>
        /// <value>The redis.</value>
        public string Redis { get; set; } = default!;

        /// <summary>
        /// Gets or sets the aurora database.
        /// </summary>
        /// <value>The aurora database.</value>
        public string AuroraDB { get; set; } = default!;
    }
}