using System.Diagnostics.CodeAnalysis;

namespace Lib.Athena.Models
{
    /// <summary>
    /// AthenaQueryConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AthenaQueryConfig
    {
        /// <summary>
        /// Gets or sets the name of the catalog.
        /// </summary>
        /// <value>The name of the catalog.</value>
        public string CatalogName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the maximum results.
        /// </summary>
        /// <value>The maximum results.</value>
        public int MaxResults { get; set; }

        /// <summary>
        /// Gets or sets the output location.
        /// </summary>
        /// <value>The output location.</value>
        public string OutputLocation { get; set; } = default!;

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>The retry count.</value>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the retry maximum delay.
        /// </summary>
        /// <value>The retry maximum delay.</value>
        public TimeSpan RetryMaxDelay { get; set; }

        /// <summary>
        /// Gets or sets the retry minimum delay.
        /// </summary>
        /// <value>The retry minimum delay.</value>
        public TimeSpan RetryMinDelay { get; set; }

        /// <summary>
        /// Gets or sets the status polling delay.
        /// </summary>
        /// <value>The status polling delay.</value>
        public int StatusPollingDelay { get; set; }

        /// <summary>
        /// Gets or sets the name of the workgroup.
        /// </summary>
        /// <value>The name of the workgroup.</value>
        public string WorkgroupName { get; set; } = default!;
    }
}