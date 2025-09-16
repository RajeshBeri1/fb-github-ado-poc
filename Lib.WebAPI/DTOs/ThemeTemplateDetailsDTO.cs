using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ThemeTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeTemplateDetailsDTO : ThemeTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public ThemeDefinition Definition { get; set; } = default!;
    }
}