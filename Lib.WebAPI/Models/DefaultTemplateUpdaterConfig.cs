using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// DefaultTemplateUpdaterConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultTemplateUpdaterConfig
    {
        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the UpdateDefaultTemplate.
        /// </summary>
        /// <value>The UpdateDefaultTemplate.</value>
        public bool UpdateDefaultTemplate { get; set; }

        /// <summary>
        /// Gets or sets a user email for default templates.
        /// </summary>
        /// <value>The UserEmail.</value>
        public string? UserEmail { get; set; }
    }
}
