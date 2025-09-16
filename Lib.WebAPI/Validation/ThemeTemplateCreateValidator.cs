using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// ThemeTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ThemeTemplateCreateValidator : AbstractValidator<ThemeTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeTemplateCreateValidator" />
        /// class.
        /// </summary>
        public ThemeTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new ThemeDefinitionValidator());
        }
    }
}