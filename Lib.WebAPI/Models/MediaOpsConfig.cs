using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// MediaOpsConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaOpsConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether [client identifier check].
        /// </summary>
        /// <value>
        /// <c>true</c> if [client identifier check]; otherwise, <c>false</c>.
        /// </value>
        public bool ClientIdCheck { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OmniAuthConfig" /> is
        /// enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the flowchart project identifier.
        /// </summary>
        /// <value>The flowchart project identifier.</value>
        public string FlowchartProjectId { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether [live check].
        /// </summary>
        /// <value><c>true</c> if [live check]; otherwise, <c>false</c>.</value>
        public bool LiveCheck { get; set; }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>The retry count.</value>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the retry delay.
        /// </summary>
        /// <value>The retry delay.</value>
        public TimeSpan RetryDelay { get; set; }

        /// <summary>
        /// Gets or sets the Environment identifier.
        /// </summary>
        /// <value>The fEnvironment identifier.</value>
        public string Environment { get; set; } = "dev"!;
    }
}
