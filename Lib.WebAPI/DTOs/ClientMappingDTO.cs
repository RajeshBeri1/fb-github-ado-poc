using Lib.WebAPI.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// ClientMappingDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientMappingDTO : DbModelBase
    {
        /// <summary>
        /// Gets or sets the ClientId identifier.
        /// </summary>
        /// <value>The ClientId identifier.</value>
        public Guid ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        /// <value>The Version.</value>
        [Range(1, 10)]
        public int Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the UseParentFlightDates identifier.
        /// </summary>
        /// <value>The Use Parent FlightDates identifier.</value>
        [DefaultValue(false)]
        public bool UseParentFlightDates { get; set; } = false;
    }
}
