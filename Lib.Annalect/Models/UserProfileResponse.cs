using System.Diagnostics.CodeAnalysis;

namespace Lib.Annalect.Models
{
    /// <summary>
    /// UserProfileResponse
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserProfileResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance can impersonate.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can impersonate; otherwise, <c>false</c>.
        /// </value>
        public bool CanImpersonate { get; set; } = default!;

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>The clients.</value>
        public Client[] Clients { get; set; } = default!;

        /// <summary>
        /// Gets or sets the e mail.
        /// </summary>
        /// <value>The e mail.</value>
        public string EMail { get; set; } = default!;

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        public string Environment { get; set; } = default!;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is require EULA and data
        /// policy prompt.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is require EULA and data policy prompt;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsRequireEulaAndDataPolicyPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is sa.
        /// </summary>
        /// <value><c>true</c> if this instance is sa; otherwise, <c>false</c>.</value>
        public bool IsSA { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the logon.
        /// </summary>
        /// <value>The name of the logon.</value>
        public string LogonName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the org.
        /// </summary>
        /// <value>The name of the org.</value>
        public string OrgName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the person identifier.
        /// </summary>
        /// <value>The person identifier.</value>
        public string PersonId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        /// <value>The projects.</value>
        public Project[] Projects { get; set; } = default!;

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public Role[] Roles { get; set; } = default!;

        /// <summary>
        /// Gets or sets the terms identifier current.
        /// </summary>
        /// <value>The terms identifier current.</value>
        public string TermsIdCurrent { get; set; } = default!;

        /// <summary>
        /// Gets or sets the theme key.
        /// </summary>
        /// <value>The theme key.</value>
        public string ThemeKey { get; set; } = default!;

        /*
        /// <summary>
        /// Gets or sets the user image.
        /// </summary>
        /// <value>The user image.</value>
        public string? UserImage { get; set; }
        */
    }
}