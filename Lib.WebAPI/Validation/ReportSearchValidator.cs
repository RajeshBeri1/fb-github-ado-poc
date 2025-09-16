using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// ReportSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReportSearchValidator : NamedSearchValidator<ReportSearchDTO, Report>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportSearchValidator" /> class.
        /// </summary>
        public ReportSearchValidator()
            : base()
        {
            RuleFor(x => x.FlowchartTemplateId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}