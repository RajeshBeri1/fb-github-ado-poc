using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// DbModelBase
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class DbModelBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DbModelBase" /> is
        /// removed.
        /// </summary>
        /// <value><c>true</c> if removed; otherwise, <c>false</c>.</value>
        public bool Removed { get; set; }
    }
}