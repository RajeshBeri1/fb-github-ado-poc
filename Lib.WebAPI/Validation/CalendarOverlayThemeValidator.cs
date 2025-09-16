using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlayTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayThemeValidator : AbstractValidator<CalendarOverlayTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOverlayThemeValidator" />
        /// class.
        /// </summary>
        public CalendarOverlayThemeValidator()
        {
            // TODO
        }
    }
}