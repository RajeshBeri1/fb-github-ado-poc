using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ClientInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientInfoDTO
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>The ClientId.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IncludeSourceMediaToolsData identifier.
        /// </summary>
        /// <value>The IncludeSourceMediaToolsData identifier.</value>
        [DefaultValue(true)]
        public bool IncludeSourceMediaToolsData { get; set; } = true;
    }
}