using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FlowchartTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateCreateValidator : AbstractValidator<FlowchartTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplateCreateValidator"
        /// /> class.
        /// </summary>
        public FlowchartTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.TemplateName).NotEmpty();
            RuleFor(x => x.Definition!).SetValidator(new FlowchartDefinitionValidator());
        }
    }
}