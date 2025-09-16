using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.DbModels
{
    /// <summary>
    /// DefaultTemplate
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultTemplate : NamedDbModelBase
    {
        /// <summary>
        /// Gets or sets the omni client identifier.
        /// </summary>
        /// <value>The omni client identifier.</value>
        public Guid OmniClientId { get; set; } = default!;
    }
}
