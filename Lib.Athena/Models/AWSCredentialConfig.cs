using System.Diagnostics.CodeAnalysis;

namespace Lib.Athena.Models
{
    /// <summary>
    /// AWSCredentialConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AWSCredentialConfig
    {
        /// <summary>
        /// Gets or sets the access key.
        /// </summary>
        /// <value>The access key.</value>
        public string AccessKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the duration seconds.
        /// </summary>
        /// <value>The duration seconds.</value>
        public int DurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        public string ExternalId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the role arn.
        /// </summary>
        /// <value>The role arn.</value>
        public string RoleArn { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the role session.
        /// </summary>
        /// <value>The name of the role session.</value>
        public string RoleSessionName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        /// <value>The secret key.</value>
        public string SecretKey { get; set; } = default!;
    }
}