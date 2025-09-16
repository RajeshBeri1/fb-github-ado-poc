using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Alignment
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Alignment
    {
        /// <summary>
        /// Gets or sets the horizontal.
        /// </summary>
        /// <value>The horizontal.</value>
        public HorizontalAlignment? Horizontal { get; set; } = null;

        /// <summary>
        /// Gets or sets the vertical.
        /// </summary>
        /// <value>The vertical.</value>
        public VerticalAlignment? Vertical { get; set; } = null;
    }
}