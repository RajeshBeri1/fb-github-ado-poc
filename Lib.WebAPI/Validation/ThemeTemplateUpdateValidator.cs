using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// ThemeTemplateUpdateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeTemplateUpdateValidator : AbstractValidator<ThemeTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeTemplateUpdateValidator" />
        /// class.
        /// </summary>
        public ThemeTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new ThemeDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}