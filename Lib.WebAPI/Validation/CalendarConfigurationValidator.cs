using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarConfigurationValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarConfigurationValidator : AbstractValidator<CalendarConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarConfigurationValidator"
        /// /> class.
        /// </summary>
        public CalendarConfigurationValidator()
        {
            RuleFor(x => x.IsReportingTimeFrame).NotNull();
            RuleFor(x => x.ReportingTimeFrame).NotNull().When(x => x.IsReportingTimeFrame);
            RuleFor(x => x.CustomEndDate).NotNull().When(x => !x.IsReportingTimeFrame);
            RuleFor(x => x.CustomStartDate).NotNull().When(x => !x.IsReportingTimeFrame);
            RuleFor(x => x.Type).NotNull();
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).SetValidator(new CalendarRowValidator());
        }
    }
}