using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Enumerations
{
    /// <summary>
    /// flow chart component
    /// </summary>
    public enum FlowChartComponent
    {
        /// <summary>
        /// The None
        /// </summary>
        None = 0,

        /// <summary>
        /// The header
        /// </summary>
        Header = 1,

        /// <summary>
        /// The calender
        /// </summary>
        Calendar = 2,

        /// <summary>
        /// The calender overlay
        /// </summary>
        CalendarOverlay = 3,

        /// <summary>
        /// The media hierarchy
        /// </summary>
        MediaHierarchy = 4,

        /// <summary>
        /// The grand totals
        /// </summary>
        GrandTotals = 5,

        /// <summary>
        /// The right hand totals
        /// </summary>
        RightHandTotals = 6,

        /// <summary>
        /// The theme
        /// </summary>
        Theme = 7,

        /// <summary>
        /// The footer
        /// </summary>
        Footer = 8,
    }
}
