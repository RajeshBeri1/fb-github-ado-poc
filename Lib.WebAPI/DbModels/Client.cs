using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// Client
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Client : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the omni clients.
        /// </summary>
        /// <value>The omni clients.</value>
        public virtual ICollection<OmniClient> OmniClients { get; set; } = new Collection<OmniClient>();

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IncludeSourceMediaToolsData identifier.
        /// </summary>
        /// <value>The IncludeSourceMediaToolsData identifier.</value>
        [DefaultValue(true)]
        public bool IncludeSourceMediaToolsData { get; set; } = true;
    }
}