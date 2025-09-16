using System.Diagnostics.CodeAnalysis;

namespace Lib.Annalect.Models
{
    /// <summary>
    /// Client
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Client
    {
        /// <summary>
        /// Gets or sets the accessed when.
        /// </summary>
        /// <value>The accessed when.</value>
        public string AccessedWhen { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = default!;

        /*
        /// <summary>
        /// Gets or sets the client identifier parent.
        /// </summary>
        /// <value>The client identifier parent.</value>
        public string? ClientIdParent { get; set; }
        */

        /*
        /// <summary>
        /// Gets or sets the client name parent.
        /// </summary>
        /// <value>The client name parent.</value>
        public string? ClientNameParent { get; set; } = default!;
        */

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy { get; set; } = default!;

        /// <summary>
        /// Gets or sets the unique identifier person created.
        /// </summary>
        /// <value>The unique identifier person created.</value>
        public string GuidPersonCreated { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hide logo.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hide logo; otherwise, <c>false</c>.
        /// </value>
        public bool IsHideLogo { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is parent.
        /// </summary>
        /// <value><c>true</c> if this instance is parent; otherwise, <c>false</c>.</value>
        public bool IsParent { get; set; }

        /// <summary>
        /// Gets or sets the omni navigation key.
        /// </summary>
        /// <value>The omni navigation key.</value>
        public string OmniNavigationKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the org.
        /// </summary>
        /// <value>The name of the org.</value>
        public string OrgName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the s3 URL logo.
        /// </summary>
        /// <value>The s3 URL logo.</value>
        public string S3UrlLogo { get; set; } = default!;

        /// <summary>
        /// Gets or sets the start when.
        /// </summary>
        /// <value>The start when.</value>
        public string StartWhen { get; set; } = default!;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URL logo.
        /// </summary>
        /// <value>The URL logo.</value>
        public string UrlLogo { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URL logo header.
        /// </summary>
        /// <value>The URL logo header.</value>
        public string UrlLogoHeader { get; set; } = default!;
    }
}