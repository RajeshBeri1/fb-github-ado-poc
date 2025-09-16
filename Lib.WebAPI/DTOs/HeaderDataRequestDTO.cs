using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// HeaderDataRequestDTO
    /// </summary>
    public class HeaderDataRequestDTO
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

        ///// <summary>
        ///// Gets or sets the currency.
        ///// </summary>
        ///// <value>The currency.</value>
        // public string Currency { get; set; } = default!;
    }
}