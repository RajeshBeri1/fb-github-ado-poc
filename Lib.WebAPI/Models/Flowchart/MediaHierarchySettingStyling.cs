using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySettingStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySettingStyling
    {
        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public MediaHierarchySettingAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor { get; set; } = default!;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font { get; set; } = default!;
    }
}