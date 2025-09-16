using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarTemplateCreateValidator : AbstractValidator<CalendarTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarTemplateCreateValidator"
        /// /> class.
        /// </summary>
        public CalendarTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new CalendarDefinitionValidator());
        }
    }
}