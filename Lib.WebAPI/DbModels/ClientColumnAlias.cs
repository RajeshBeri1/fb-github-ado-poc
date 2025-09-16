using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// ClientColumnAlias
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientColumnAlias : DbModelBase
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the aliases.
        /// </summary>
        /// <value>The aliases.</value>
        public string ColumnAliases { get; set; } = default!;
    }
}