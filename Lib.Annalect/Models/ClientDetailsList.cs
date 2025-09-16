using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Annalect.Models
{
    /// <summary>
    /// ClientDetailsList
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ClientDetailsList
    {
        /// <summary>
        /// Gets or sets the ClientId.
        /// </summary>
        /// <value>
        /// The ClientId.
        /// </value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>
        /// The Name.
        /// </value>
        public string Name { get; set; }
    }
}