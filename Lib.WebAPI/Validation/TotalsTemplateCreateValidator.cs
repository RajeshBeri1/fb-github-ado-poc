using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// TotalsTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsTemplateCreateValidator : AbstractValidator<TotalsTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TotalsTemplateCreateValidator" />
        /// class.
        /// </summary>
        public TotalsTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new TotalsDefinitionValidator());
        }
    }
}