using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlayRow
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayRowValidator : AbstractValidator<CalendarOverlayRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOverlayRowValidator" />
        /// class.
        /// </summary>
        public CalendarOverlayRowValidator()
        {
            RuleFor(x => x.EndDate).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}