using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GrandTotalSelection
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalSelectionValidator : AbstractValidator<GrandTotalSelection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrandTotalSelectionValidator" />
        /// class.
        /// </summary>
        public GrandTotalSelectionValidator()
        {
            RuleFor(x => x.FlightRange).NotNull();
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Order).NotNull();
        }
    }
}