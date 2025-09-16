using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlayTemplateCreateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTemplateCreateValidator : AbstractValidator<CalendarOverlayTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlayTemplateCreateValidator" /> class.
        /// </summary>
        public CalendarOverlayTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new CalendarOverlayDefinitionValidator());
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}