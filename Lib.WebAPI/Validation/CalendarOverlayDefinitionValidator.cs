using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlayDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayDefinitionValidator : AbstractValidator<CalendarOverlayDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlayDefinitionValidator" /> class.
        /// </summary>
        public CalendarOverlayDefinitionValidator()
        {
            RuleFor(x => x.OverlaySections).NotEmpty();
            RuleForEach(x => x.OverlaySections).SetValidator(new CalendarOverlaySectionValidator());
        }
    }
}