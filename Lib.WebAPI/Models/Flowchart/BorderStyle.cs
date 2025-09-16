using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// BorderStyle
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BorderStyle
    {
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color { get; set; } = null!;

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public BorderLineStyle? Style { get; set; } = null;
    }
}