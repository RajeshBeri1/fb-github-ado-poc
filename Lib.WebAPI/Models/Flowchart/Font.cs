using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Font
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Font
    {
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Color? Background { get; set; } = null;

        /// <summary>
        /// Gets or sets the background alternate.
        /// </summary>
        /// <value>The background alternate.</value>
        public Color? BackgroundAlternate { get; set; } = null;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color? Color { get; set; } = null;

        /// <summary>
        /// Gets or sets the color alternate.
        /// </summary>
        /// <value>The color alternate.</value>
        public Color? ColorAlternate { get; set; } = null;

        /// <summary>
        /// Gets or sets the family.
        /// </summary>
        /// <value>The family.</value>
        public string? Family { get; set; } = null;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int? Size { get; set; } = null;

        /// <summary>
        /// Gets or sets the underline style
        /// </summary>
        /// <value>The underline style.</value>
        public RangeUnderlineStyle? Underline { get; set; } = null;

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public FontWeight? Weight { get; set; } = null;
    }
}