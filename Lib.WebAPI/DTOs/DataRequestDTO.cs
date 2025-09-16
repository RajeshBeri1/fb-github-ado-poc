using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DataRequestDTO
    /// </summary>
    public class DataRequestDTO
    {
        /// <summary>
        /// Gets or sets the flowchart definition.
        /// </summary>
        /// <value>The flowchart definition.</value>
        public FlowchartDefinition FlowchartDefinition { get; set; } = default!;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public Guid OmniClientId { get; set; }

        /// <summary>
        /// Gets or sets the run restrictions.
        /// </summary>
        /// <value>The run restrictions.</value>
        public ICollection<RunRestriction> RunRestrictions { get; set; } = default!;

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        public string? Currency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the BriefedCTC.
        /// </summary>
        /// <value>The BriefedCTC.</value>
        public bool ShowBriefedCTC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the UseStaticFilter.
        /// </summary>
        /// <value>The UseStaticFilter.</value>
        public bool UseStaticFilter { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the PreviousData.
        /// </summary>
        /// <value>The PreviousData.</value>
        public ICollection<Dictionary<string, object?>>? PreviousData { get; set; } = null;
    }
}