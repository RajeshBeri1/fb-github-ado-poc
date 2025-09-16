using System.Diagnostics.CodeAnalysis;

namespace Lib.Annalect.Models
{
    /// <summary>
    /// Role
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Role
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;
    }
}