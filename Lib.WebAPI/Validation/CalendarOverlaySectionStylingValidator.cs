using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlaySectionStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlaySectionStylingValidator : AbstractValidator<CalendarOverlaySectionStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlaySectionStylingValidator" /> class.
        /// </summary>
        public CalendarOverlaySectionStylingValidator()
        {
            RuleFor(x => x.BackgroundColor).NotEmpty().SetValidator(new ColorValidator());
            RuleFor(x => x.Format).NotNull();
        }
    }
}