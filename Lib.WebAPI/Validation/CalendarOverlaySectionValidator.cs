using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlaySection
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlaySectionValidator : AbstractValidator<CalendarOverlaySection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOverlaySectionValidator"
        /// /> class.
        /// </summary>
        public CalendarOverlaySectionValidator()
        {
            RuleFor(x => x.Order).NotNull();

            /*
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new CalendarOverlaySectionStylingValidator());
            */
            RuleFor(x => x.Text).NotEmpty();

            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).SetValidator(new CalendarOverlayRowValidator());
        }
    }
}