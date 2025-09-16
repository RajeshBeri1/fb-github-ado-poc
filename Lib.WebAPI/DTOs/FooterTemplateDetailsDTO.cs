using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// FooterTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterTemplateDetailsDTO : FooterTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public FooterDefinition Definition { get; set; } = default!;
    }
}