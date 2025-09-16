using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Models.Data;
using Lib.WebAPI.Models.Flowchart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// SaveFlowchartDTO
    /// </summary>
    [ExcludeFromCodeCoverage]

    public class SaveFlowchartDTO
    {
        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public FlowchartDefinition? Definition { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string TemplateName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        /// <value>The Version.</value>
        [DefaultValue(1)]
        public int? Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
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

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the IsDefault identifier.
        /// </summary>
        /// <value>The IsDefault identifier.</value>
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;

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
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public ICollection<RunRestriction> RunRestrictions { get; set; } = default!;

        /// <summary>
        /// Gets or sets the report publish details.
        /// </summary>
        /// <value>The report publish details.</value>
        public ReportPublishDTO? ReportPublish { get; set; } = default!;
    }
}
