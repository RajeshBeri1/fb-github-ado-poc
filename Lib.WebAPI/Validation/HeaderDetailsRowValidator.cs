using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderDetailsRow
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDetailsRowValidator : AbstractValidator<HeaderDetailsRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDetailsRowValidator" />
        /// class.
        /// </summary>
        public HeaderDetailsRowValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Text).NotEmpty();
            RuleFor(x => x.ValueSeparator).NotNull();
        }
    }
}