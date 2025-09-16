using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderTemplateUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderTemplateUpdateValidator : AbstractValidator<HeaderTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderTemplateUpdateValidator" />
        /// class.
        /// </summary>
        public HeaderTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new HeaderDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}