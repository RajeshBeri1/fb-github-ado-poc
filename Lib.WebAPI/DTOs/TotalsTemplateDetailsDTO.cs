using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// TotalsTemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsTemplateDetailsDTO : TotalsTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public TotalsDefinition Definition { get; set; } = default!;
    }
}