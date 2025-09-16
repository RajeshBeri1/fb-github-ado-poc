using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// TemplateUpdateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateUpdateDTO
    {
        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public FlowchartDefinition? Definition { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the flowchartTemplatesVersionHistorties comments.
        /// </summary>
        /// <value>The flowchartTemplatesVersionHistorties comments.</value>
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
    }
}