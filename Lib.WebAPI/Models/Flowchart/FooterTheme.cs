using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// HeaderTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterTheme
    {
        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}