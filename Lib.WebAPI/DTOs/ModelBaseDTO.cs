using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ModelBaseDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ModelBaseDTO" /> is
        /// removed.
        /// </summary>
        /// <value><c>true</c> if removed; otherwise, <c>false</c>.</value>
        public bool Removed { get; set; }
    }
}