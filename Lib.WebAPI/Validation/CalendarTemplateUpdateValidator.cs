using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarTemplateUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarTemplateUpdateValidator : AbstractValidator<CalendarTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarTemplateUpdateValidator"
        /// /> class.
        /// </summary>
        public CalendarTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new CalendarDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}