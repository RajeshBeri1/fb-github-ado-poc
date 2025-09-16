using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FlowchartTemplateUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateUpdateValidator : AbstractValidator<FlowchartTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplateUpdateValidator"
        /// /> class.
        /// </summary>
        public FlowchartTemplateUpdateValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Definition!).SetValidator(new FlowchartDefinitionValidator());
            RuleFor(x => x.Comments).NotEmpty();

        }
    }
}