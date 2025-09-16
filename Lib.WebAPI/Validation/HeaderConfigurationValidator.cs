using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderConfigurationValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderConfigurationValidator : AbstractValidator<HeaderConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderConfigurationValidator" />
        /// class.
        /// </summary>
        public HeaderConfigurationValidator()
        {
            RuleFor(x => x.ColumnMargin).NotNull();
            RuleFor(x => x.RowMargin).NotNull();
            RuleFor(x => x.RowMerge).NotNull();

            RuleFor(x => x.Details!).SetValidator(new HeaderDetailsValidator());

            RuleForEach(x => x.Logos).SetValidator(new HeaderLogoValidator());

            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).SetValidator(new HeaderRowValidator());
        }
    }
}