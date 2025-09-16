using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarRowStylingValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarRowStylingValidator : AbstractValidator<CalendarRowStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarRowStylingValidator" />
        /// class.
        /// </summary>
        public CalendarRowStylingValidator()
        {
            RuleFor(x => x.BackgroundColor).NotNull().SetValidator(new ColorValidator());
            RuleFor(x => x.AlternateBackgroundColor!).SetValidator(new ColorValidator());
        }
    }
}