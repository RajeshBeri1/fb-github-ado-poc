using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// TotalsDefinitionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsDefinitionValidator : AbstractValidator<TotalsDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TotalsDefinitionValidator" />
        /// class.
        /// </summary>
        public TotalsDefinitionValidator()
        {
            RuleFor(x => x.Columns).NotEmpty();
            RuleForEach(x => x.Columns).SetValidator(new TotalsColumnValidator());
        }
    }
}