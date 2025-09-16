using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// RunRestrictionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunRestrictionValidator : AbstractValidator<RunRestriction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunRestrictionValidator" />
        /// class.
        /// </summary>
        public RunRestrictionValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.ValueJson).NotEmpty();
        }
    }
}