using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// Position
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Precision
    {
        /// <summary>
        /// Gets or sets the precisionValue.
        /// </summary>
        /// <value>The precisionValue.</value>
        public string? PrecisionValue { get; set; } = null;

        /// <summary>
        /// Gets or sets the applyToCurrencyMetric.
        /// </summary>
        /// <value>The applyToCurrencyMetric.</value>
        public bool? ApplyToCurrencyMetric { get; set; } = true;

        /// <summary>
        /// Gets or sets the applyToNonCurrencyMetric.
        /// </summary>
        /// <value>The applyToNonCurrencyMetric.</value>
        public bool? ApplyToNonCurrencyMetric { get; set; } = true;
    }
}
