using Lib.WebAPI.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DefaultTemplateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultTemplateDTO
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the omni client identifier.
        /// </summary>
        /// <value>The omni client identifier.</value>
        public Guid OmniClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DbModelBase" /> is
        /// removed.
        /// </summary>
        /// <value><c>true</c> if removed; otherwise, <c>false</c>.</value>
        public bool Removed { get; set; }
    }
}
