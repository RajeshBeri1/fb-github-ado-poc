using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Data
{
    /// <summary>
    /// FlowchartDataMetric
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDataMetric
    {
        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        /// <value>The effective date.</value>
        public DateOnly EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the flight end.
        /// </summary>
        /// <value>The flight end.</value>
        public DateOnly FlightEnd { get; set; }

        /// <summary>
        /// Gets or sets the flight start.
        /// </summary>
        /// <value>The flight start.</value>
        public DateOnly FlightStart { get; set; }

        /// <summary>
        /// Gets or sets the inflight overlay value json.
        /// </summary>
        /// <value>The inflight overlay value json.</value>
        public string? InflightOverlayValueJson { get; set; }

        /// <summary>
        /// Gets or sets the value json.
        /// </summary>
        /// <value>The value json.</value>
        public string ValueJson { get; set; } = default!;

        /// <summary>
        /// Gets or sets the value json.
        /// </summary>
        /// <value>The value json.</value>
        public List<string> InflightOverlays { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the value json.
        /// </summary>
        /// <value>The value json.</value>
        public string? Legend { get; set; }

        /// <summary>
        /// Gets or sets the CompositeKey.
        /// </summary>
        /// <value>The CompositeKey.</value>
        public string? CompositeKey { get; set; }
    }
}