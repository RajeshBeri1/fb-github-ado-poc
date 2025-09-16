using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// FooterDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterDefinition
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public FooterConfiguration Configuration { get; set; } = default!;

        /// <summary>
        /// Gets or sets the styling.
        /// </summary>
        /// <value>The styling.</value>
        public Styling Styling { get; set; } = default!;
    }
}