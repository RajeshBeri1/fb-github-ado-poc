using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FooterRowValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterRowValidator : AbstractValidator<FooterRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooterRowValidator" /> class.
        /// </summary>
        public FooterRowValidator()
        {
            RuleFor(x => x.Order).NotNull();
            // RuleFor(x => x.Text).NotEmpty().When(x => x.Type == FooterRowType.Text);
            // RuleFor(x => x.Styling).NotEmpty().SetValidator(new FooterRowStylingValidator());
        }
    }
}