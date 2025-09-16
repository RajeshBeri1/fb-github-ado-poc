using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryDefinitionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryDefinitionValidator : AbstractValidator<SummaryDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryDefinitionValidator" />
        /// class.
        /// </summary>
        public SummaryDefinitionValidator()
        {
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).SetValidator(new SummaryRowValidator());
        }
    }
}