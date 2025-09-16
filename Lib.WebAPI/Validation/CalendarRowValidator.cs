using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarRowValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarRowValidator : AbstractValidator<CalendarRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarRowValidator" /> class.
        /// </summary>
        public CalendarRowValidator()
        {
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.Style).NotNull();
            // RuleFor(x => x.Styling).NotEmpty().SetValidator(new CalendarRowStylingValidator());
        }
    }
}