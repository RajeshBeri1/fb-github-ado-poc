using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Position
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Position
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public string? Start { get; set; } = null;
    }
}