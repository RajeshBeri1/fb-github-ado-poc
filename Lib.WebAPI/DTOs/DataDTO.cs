using System.Diagnostics.CodeAnalysis;

namespace Lib.WebAPI.DTOs
{
    /// <summary>
    /// DataDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataDTO 
    {
        /// <summary>
        /// Gets or sets the Column1.
        /// </summary>
        /// <value>The Id.</value>
        public string Column1 { get; set; } = default!;

        /// <summary>
        /// Gets or sets the Column2.
        /// </summary>
        /// <value>The Id.</value>
        public string Column2 { get; set; } = default!;
    }
}