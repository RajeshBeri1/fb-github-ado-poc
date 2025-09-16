using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Styling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Styling
    {
        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public Alignment Alignment { get; set; } = default!;

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public Border Border { get; set; } = default!;

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>The fill.</value>
        public Fill Fill { get; set; } = default!;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font { get; set; } = default!;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Position? Position { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>The precision.</value>
        public Precision? Precision { get; set; }

        /// <summary>
        /// Gets or sets the showbreiefedctc.
        /// </summary>
        /// <value>The showbreifedctc.</value>
        public ShowBriefedCTC? ShowBriefedCTC { get; set; }

        /// <summary>
        /// Gets or sets the customstylesetting.
        /// </summary>
        /// <value>The customstylesetting.</value>
        public CustomStyleSetting? CustomStyleSetting { get; set; }
    }
}