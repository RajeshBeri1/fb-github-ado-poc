using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyDefinition
    {
        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        /// <value>The levels.</value>
        public ICollection<MediaHierarchyLevel> Levels { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}