using System.Diagnostics.CodeAnalysis;

namespace Lib.Annalect.Models
{
    /// <summary>
    /// ValidateSessionResponse
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ValidateSessionResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is authorized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthorized { get; set; }

        /// <summary>
        /// Gets or sets the person identifier.
        /// </summary>
        /// <value>The person identifier.</value>
        public string PersonId { get; set; } = default!;
    }
}