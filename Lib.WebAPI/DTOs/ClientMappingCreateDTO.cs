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
    /// ClientMappingCreateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientMappingCreateDTO
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
        [DefaultValue(1)]
        public int? Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DbModelBase" /> is
        /// removed.
        /// </summary>
        /// <value><c>true</c> if removed; otherwise, <c>false</c>.</value>
        public bool Removed { get; set; }
    }
}
