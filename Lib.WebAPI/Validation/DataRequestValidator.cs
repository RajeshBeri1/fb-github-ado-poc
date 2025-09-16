using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// DataRequestValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataRequestValidator : AbstractValidator<DataRequestDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRequestValidator" /> class.
        /// </summary>
        public DataRequestValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.FlowchartDefinition).NotNull().SetValidator(new FlowchartDefinitionValidator());
            RuleFor(x => x.RunRestrictions).NotNull();
            RuleForEach(x => x.RunRestrictions).SetValidator(new RunRestrictionValidator());
        }
    }
}