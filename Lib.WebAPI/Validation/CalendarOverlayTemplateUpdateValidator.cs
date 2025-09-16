using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlayTemplateUpdateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTemplateUpdateValidator : AbstractValidator<CalendarOverlayTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlayTemplateUpdateValidator" /> class.
        /// </summary>
        public CalendarOverlayTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new CalendarOverlayDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}