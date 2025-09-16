using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryTemplateCreateValidator : AbstractValidator<SummaryTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryTemplateCreateValidator"
        /// /> class.
        /// </summary>
        public SummaryTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new SummaryDefinitionValidator());
        }
    }
}