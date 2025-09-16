using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// GrandTotalTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalTemplateDetailsDTO : GrandTotalTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public GrandTotalDefinition Definition { get; set; } = default!;
    }
}