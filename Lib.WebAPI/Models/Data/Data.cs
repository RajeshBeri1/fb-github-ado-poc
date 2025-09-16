using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// Data
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Data
    {
        /// <summary>
        /// Gets or sets the flowchart data.
        /// </summary>
        /// <value>The flowchart data.</value>
        public FlowchartData? FlowchartData { get; set; }

        /// <summary>
        /// Gets or sets the header data.
        /// </summary>
        /// <value>The header data.</value>
        public HeaderData? HeaderData { get; set; }

        /// <summary>
        /// Gets or sets the summary data.
        /// </summary>
        /// <value>The summary data.</value>
        public SummaryData? SummaryData { get; set; }
    }
}