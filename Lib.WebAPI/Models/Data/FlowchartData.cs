using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// FlowchartData
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartData
    {
        /// <summary>
        /// Gets or sets the grand totals.
        /// </summary>
        /// <value>The grand totals.</value>
        public ICollection<GrandTotalData>? GrandTotals { get; set; }

        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        /// <value>The levels.</value>
        public ICollection<FlowchartDataLevel> Levels { get; set; } = default!;

        /// <summary>
        /// Gets or sets the RawData.
        /// </summary>
        /// <value>The RawData.</value>
        public ICollection<Dictionary<string, object?>>? RawData { get; set; } = null;
    }
}