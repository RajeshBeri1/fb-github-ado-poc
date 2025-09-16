using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryTemplateUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryTemplateUpdateValidator : AbstractValidator<SummaryTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryTemplateUpdateValidator"
        /// /> class.
        /// </summary>
        public SummaryTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new SummaryDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}