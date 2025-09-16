using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderRowValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderRowValidator : AbstractValidator<HeaderRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderRowValidator" /> class.
        /// </summary>
        public HeaderRowValidator()
        {
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.Type).NotNull();
            RuleFor(x => x.Date).NotEmpty().When(x => x.Type == HeaderRowType.Date);
            RuleFor(x => x.DateFormat).NotNull().When(x => x.Type == HeaderRowType.Date);
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty).When(x => x.Type == HeaderRowType.Value);
            RuleFor(x => x.ColumnName).NotEmpty().When(x => x.Type == HeaderRowType.Value);
            RuleFor(x => x.ValueSeparator).NotNull().When(x => x.Type == HeaderRowType.Value);
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
            RuleFor(x => x.Text).NotEmpty().When(x => x.Type == HeaderRowType.Text);
        }
    }
}