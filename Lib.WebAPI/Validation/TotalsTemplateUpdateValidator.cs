using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// TotalsTemplateUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsTemplateUpdateValidator : AbstractValidator<TotalsTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TotalsTemplateUpdateValidator" />
        /// class.
        /// </summary>
        public TotalsTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new TotalsDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}