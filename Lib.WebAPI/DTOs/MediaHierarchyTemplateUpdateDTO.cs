using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Enums;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// MediaHierarchyTemplateUpdateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyTemplateUpdateDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public MediaHierarchyDefinition Definition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        public string Currency { get; set; } = default!;

        /// <summary>
        /// Gets or sets the showsubtotalsatbottom.
        /// </summary>
        /// <value>The showsubtotalsatbottom.</value>
        public bool? ShowSubTotalsAtBottom { get; set; }

        /// <summary>
        /// Gets or sets the displaySource.
        /// </summary>
        /// <value>The displaySource.</value>
        public string? DisplaySource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}