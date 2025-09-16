using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// StorageConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StorageConfig
    {
        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        /// <value>The name of the container.</value>
        public string ContainerName { get; set; } = default!;
    }
}