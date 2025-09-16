using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// NamedModelBaseDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class NamedModelBaseDTO : ModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;
    }
}