using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// NamedDbModelBase
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class NamedDbModelBase : DbModelBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; } = default!;
    }
}