using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryRowValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryRowValidator : AbstractValidator<SummaryRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryRowValidator" /> class.
        /// </summary>
        public SummaryRowValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}