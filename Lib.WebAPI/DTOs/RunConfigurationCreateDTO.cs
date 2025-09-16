using Lib.WebAPI.Models.Flowchart;
using System.ComponentModel.DataAnnotations;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// RunConfigurationCreateDTO
    /// </summary>
    public class RunConfigurationCreateDTO
    {
        /// <summary>
        /// Gets or sets the flowchart template identifier.
        /// </summary>
        /// <value>The flowchart template identifier.</value>
        public Guid FlowchartTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the calendar definition.
        /// </summary>
        /// <value>The calendar definition.</value>
        public CalendarTemplateDetailsDTO CalendarDefinition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the media hierarchy levels.
        /// </summary>
        /// <value>The media hierarchy levels.</value>
        public ICollection<MediaHierarchyLevelBase> MediaHierarchyLevels { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public ICollection<RunRestriction> RunRestrictions { get; set; } = default!;

        /// <summary>
        /// Gets or sets the run configuration comments.
        /// </summary>
        /// <value>The run configuration comments.</value>
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets the Custom Style Settings.
        /// </summary>
        /// <value>The Custom Style Settings.</value>
        public string? CustomStyleSettings { get; set; }

        /// <summary>
        /// Gets or sets the Track Changes.
        /// </summary>
        /// <value>The Track Format Changes.</value>
        public bool? TrackFormatChanges { get; set; }
    }
}