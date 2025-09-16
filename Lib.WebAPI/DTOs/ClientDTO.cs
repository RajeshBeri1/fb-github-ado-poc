using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ClientInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientDTO : NamedModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The Id.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IncludeSourceMediaToolsData identifier.
        /// </summary>
        /// <value>The IncludeSourceMediaToolsData identifier.</value>
        [DefaultValue(true)]
        public bool? IncludeSourceMediaToolsData { get; set; }
    }
}