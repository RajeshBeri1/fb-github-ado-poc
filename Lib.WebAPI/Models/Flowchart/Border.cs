using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Border
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Border
    {
        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public BorderStyle Bottom { get; set; } = default!;

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public BorderStyle Left { get; set; } = default!;

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public BorderStyle Right { get; set; } = default!;

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public BorderStyle Top { get; set; } = default!;
    }
}