using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models
{
    /// <summary>
    /// DataDictionaryUpdaterConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDictionaryUpdaterConfig
    {
        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public TimeSpan Interval { get; set; }
    }
}