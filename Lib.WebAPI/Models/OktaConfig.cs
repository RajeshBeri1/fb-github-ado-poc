using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// OktaAuthentication
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OktaConfig
    {
        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OktaConfig" /> is
        /// enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
    }
}