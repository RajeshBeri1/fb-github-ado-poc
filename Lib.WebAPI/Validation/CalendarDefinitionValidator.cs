using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarDefinitionValidation
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarDefinitionValidator : AbstractValidator<CalendarDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarDefinitionValidator" />
        /// class.
        /// </summary>
        public CalendarDefinitionValidator()
        {
            RuleFor(x => x.Configuration).NotEmpty().SetValidator(new CalendarConfigurationValidator());
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
        }
    }
}