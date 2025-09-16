using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// CalendarOverlayTemplateCreateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTemplateCreateDTO
    {
        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public CalendarOverlayDefinition Definition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}