using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// OmniClientInfoDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OmniClientInfoDTO : ModelBaseDTO
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        public ClientInfoDTO Client { get; set; } = default!;

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; } = default!;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        /// <value>The Version.</value>
        [DefaultValue(1)]
        public int? Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the UseParentFlightDates identifier.
        /// </summary>
        /// <value>The Use Parent FlightDates identifier.</value>
        [DefaultValue(false)]
        public bool UseParentFlightDates { get; set; } = false;
    }
}