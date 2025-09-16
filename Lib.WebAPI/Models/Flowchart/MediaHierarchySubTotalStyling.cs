using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchySubTotalStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalStyling
    {
        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public MediaHierarchySubTotalAlignment Alignment { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font { get; set; } = default!;
    }
}