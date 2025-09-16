using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// HeaderTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderTemplateDetailsDTO : HeaderTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public HeaderDefinition Definition { get; set; } = default!;
    }
}