using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// TotalsColumnValidator
    /// </summary>
    public class TotalsColumnValidator : AbstractValidator<TotalsColumn>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TotalsColumnValidator" /> class.
        /// </summary>
        public TotalsColumnValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}