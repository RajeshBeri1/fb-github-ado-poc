using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GrandTotalDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalDefinitionValidator : AbstractValidator<GrandTotalDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrandTotalDefinitionValidator" />
        /// class.
        /// </summary>
        public GrandTotalDefinitionValidator()
        {
            RuleFor(x => x.Selections).NotEmpty();
            RuleForEach(x => x.Selections).SetValidator(new GrandTotalSelectionValidator());
        }
    }
}