using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FooterConfigurationValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterConfigurationValidator : AbstractValidator<FooterConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooterConfigurationValidator" />
        /// class.
        /// </summary>
        public FooterConfigurationValidator()
        {
            RuleFor(x => x.ColumnMargin).NotNull();
            RuleFor(x => x.RowMargin).NotNull();
            RuleFor(x => x.RowMerge).NotNull();
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).SetValidator(new FooterRowValidator());
        }
    }
}