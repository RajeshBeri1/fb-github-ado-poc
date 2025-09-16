using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderTemplateCreateValidation
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderTemplateCreateValidator : AbstractValidator<HeaderTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderTemplateCreateValidator" />
        /// class.
        /// </summary>
        public HeaderTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new HeaderDefinitionValidator());
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}