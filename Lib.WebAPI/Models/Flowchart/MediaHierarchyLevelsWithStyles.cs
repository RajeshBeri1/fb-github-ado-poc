using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Models.Flowchart
{
    /// <summary>
    /// MediaHierarchyLevelsWithStyles
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyLevelsWithStyles
    {
        /// <summary>
        /// Gets or sets the flight bar styling.
        /// </summary>
        /// <value>The flight bar styling.</value>
        public Styling FlightBarStyling { get; set; } = default!;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }
    }
}
