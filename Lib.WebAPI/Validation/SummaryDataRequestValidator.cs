using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryDataRequestValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryDataRequestValidator : AbstractValidator<SummaryDataRequestDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryDataRequestValidator" />
        /// class.
        /// </summary>
        public SummaryDataRequestValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.SummaryDefinition).NotEmpty().SetValidator(new SummaryDefinitionValidator());
            RuleFor(x => x.CalendarConfiguration).NotEmpty().SetValidator(new CalendarConfigurationValidator());
            RuleFor(x => x.RunRestrictions).NotNull();
            RuleForEach(x => x.RunRestrictions).SetValidator(new RunRestrictionValidator());
        }
    }
}