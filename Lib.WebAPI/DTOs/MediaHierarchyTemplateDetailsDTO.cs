using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Enums;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// MediaHierarchyTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyTemplateDetailsDTO : MediaHierarchyTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public MediaHierarchyDefinition Definition { get; set; } = default!;

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
    }
}