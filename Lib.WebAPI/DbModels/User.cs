using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// User
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class User : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the allowed client guids.
        /// </summary>
        /// <value>The allowed client guids.</value>
        public string AllowedClientGuids { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; } = default!;
    }
}