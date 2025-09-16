using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Models.Flowchart.Enumerations
{
    /// <summary>
    /// Unit type of image.
    /// </summary>
    public enum UnitTypeOfImage
    {
        /// <summary>
        /// The percentage
        /// </summary>
        Percent = 0,

        /// <summary>
        /// The Pixel
        /// </summary>
        Pixel = 1,

        /// <summary>
        /// The Centimeters
        /// </summary>
        Centimeters = 2,

        /// <summary>
        /// The Inches
        /// </summary>
        Inches = 3,

        /// <summary>
        /// The EM
        /// </summary>
        EM = 4,

        /// <summary>
        /// The rem
        /// </summary>
        REM = 5,
    }
}
