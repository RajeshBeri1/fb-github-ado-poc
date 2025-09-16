using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// RunConfigurationDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationDetailsDTO : RunConfigurationInfoDTO
    {
        /// <summary>
        /// Gets or sets the calendar definition.
        /// </summary>
        /// <value>The calendar definition.</value>
        public CalendarTemplateDetailsDTO CalendarDefinition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the media hierarchy levels.
        /// </summary>
        /// <value>The media hierarchy levels.</value>
        public ICollection<MediaHierarchyLevel> MediaHierarchyLevels { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run resctrictions.
        /// </summary>
        /// <value>The run resctrictions.</value>
        public ICollection<RunRestriction> RunResctrictions { get; set; } = default!;
    }
}