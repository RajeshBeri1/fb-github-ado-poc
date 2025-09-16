using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Enums;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// TemplateDetailsDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateDetailsDTO : FlowchartTemplateInfoDTO
    {
        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public FlowchartDefinition? Definition { get; set; }

        /// <summary>
        /// Gets or sets the omni client identifier.
        /// </summary>
        /// <value>The omni client identifier.</value>
        public Guid OmniClientId { get; set; }

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
        /// Gets or sets the share status.
        /// </summary>
        /// <value>The Share Status.</value>
        public TemplateShareStatus ShareStatus { get; set; }
    }
}