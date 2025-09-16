using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// DefinitionReference
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class DefinitionReference<T>
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public T Definition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>The template identifier.</value>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the template version.
        /// </summary>
        /// <value>The template version.</value>
        [DefaultValue(1)]
        public int? TemplateVersion { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool? IsDefault { get; set; } = false;
    }
}