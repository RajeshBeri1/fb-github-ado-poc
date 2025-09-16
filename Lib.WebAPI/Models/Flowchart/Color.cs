using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Color
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Color
    {
        /// <summary>
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>The alpha.</value>
        public int? Alpha { get; set; } = null;

        /// <summary>
        /// Gets or sets the blue.
        /// </summary>
        /// <value>The blue.</value>
        public int? Blue { get; set; } = null;

        /// <summary>
        /// Gets or sets the green.
        /// </summary>
        /// <value>The green.</value>
        public int? Green { get; set; } = null;

        /// <summary>
        /// Gets or sets the red.
        /// </summary>
        /// <value>The red.</value>
        public int? Red { get; set; } = null;
    }
}