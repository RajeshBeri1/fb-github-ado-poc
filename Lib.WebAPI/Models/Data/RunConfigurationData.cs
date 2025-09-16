using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// RunConfigurationData
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationData
    {
        /// <summary>
        /// Gets or sets the media hierarchy levels.
        /// </summary>
        /// <value>The media hierarchy levels.</value>
        public ICollection<MediaHierarchyLevelBase> MediaHierarchyLevels { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public ICollection<RunRestriction> RunRestrictions { get; set; } = default!;
    }
}