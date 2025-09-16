using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// UserInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserInfoDTO : NamedModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; } = default!;
    }
}