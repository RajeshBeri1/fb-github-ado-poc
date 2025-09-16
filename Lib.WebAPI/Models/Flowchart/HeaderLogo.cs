using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// HeaderLogo
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderLogo
    {
        /// <summary>
        /// Gets or sets the logo alignment.
        /// </summary>
        /// <value>The logo alignment.</value>
        public HeaderLogoAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public Image Image { get; set; } = default!;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the Unit Type Of Image.
        /// </summary>
        /// <value>The Unit Type Of Image.</value>
        public UnitTypeOfImage? UnitTypeOfImage { get; set; }

        /// <summary>
        /// Gets or sets height of the image.
        /// </summary>
        public decimal? Height { get; set; } = default!;

        /// <summary>
        /// Gets or sets width of the image.
        /// </summary>
        public decimal? Width { get; set; } = default!;
    }
}