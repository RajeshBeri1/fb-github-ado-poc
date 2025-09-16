using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderDataRequestValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDataRequestValidator : AbstractValidator<HeaderDataRequestDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDataRequestValidator" />
        /// class.
        /// </summary>
        public HeaderDataRequestValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.FlowchartDefinition).NotEmpty().SetValidator(new FlowchartDefinitionValidator());
            RuleFor(x => x.FlowchartDefinition.HeaderDefinition).NotEmpty();
            RuleFor(x => x.FlowchartDefinition.MediaHierarchyDefinition).NotEmpty();
            RuleFor(x => x.FlowchartDefinition.CalendarDefinition).NotEmpty();
            RuleFor(x => x.RunRestrictions).NotNull();
            RuleForEach(x => x.RunRestrictions).SetValidator(new RunRestrictionValidator());
        }
    }
}