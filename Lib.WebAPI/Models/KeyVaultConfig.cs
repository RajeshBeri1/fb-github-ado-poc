using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// KeyVaultConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class KeyVaultConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KeyVaultConfig" /> is
        /// enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the reload interval.
        /// </summary>
        /// <value>The reload interval.</value>
        public TimeSpan ReloadInterval { get; set; }

        /// <summary>
        /// Gets or sets the TenantId.
        /// </summary>
        /// <value>The TenantId.</value>
        public string TenantId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the vault URI.
        /// </summary>
        /// <value>The vault URI.</value>
        public string VaultUri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>The TenantId.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Secret.
        /// </summary>
        /// <value>The TenantId.</value>
        public string ClientSecret { get; set; } = default!;
    }
}