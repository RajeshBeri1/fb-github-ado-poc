using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Data;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// DataValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataValidator : AbstractValidator<Data>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidator" /> class.
        /// </summary>
        public DataValidator()
        {
            RuleFor(x => x.FlowchartData!).SetValidator(new FlowchartDataValidator());
            RuleFor(x => x.HeaderData!).SetValidator(new HeaderDataValidator());
            RuleFor(x => x.SummaryData!).SetValidator(new SummaryDataValidator());
        }
    }
}