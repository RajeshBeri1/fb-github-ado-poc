using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarThemeValidator : AbstractValidator<CalendarTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarThemeValidator" /> class.
        /// </summary>
        public CalendarThemeValidator()
        {
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());

            // TODO

            // RuleFor(x => x.RowStyling).NotEmpty();
        }
    }
}