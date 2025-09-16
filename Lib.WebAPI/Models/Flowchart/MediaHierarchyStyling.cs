using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyMetricStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyStyling
    {
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public MediaHierarchyFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public MediaHierarchyOrientation Orientation { get; set; }
    }
}