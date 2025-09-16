using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// WebApiConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TaskpaneConfig
    {
        /// <summary>
        /// Gets or sets the state of the dock.
        /// </summary>
        /// <value>The state of the dock.</value>
        public string DockState { get; set; } = default!;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        public uint Row { get; set; }

        /// <summary>
        /// Gets or sets the store.
        /// </summary>
        /// <value>The store.</value>
        public string Store { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type of the store.
        /// </summary>
        /// <value>The type of the store.</value>
        public string StoreType { get; set; } = default!;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TaskpaneConfig" /> is
        /// visibility.
        /// </summary>
        /// <value><c>true</c> if visibility; otherwise, <c>false</c>.</value>
        public bool Visibility { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }
    }
}