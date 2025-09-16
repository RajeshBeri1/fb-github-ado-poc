using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FooterRowStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterRowStylingValidator : AbstractValidator<FooterRowStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooterRowStylingValidator" />
        /// class.
        /// </summary>
        public FooterRowStylingValidator()
        {
            RuleFor(x => x.Alignment).NotNull();
            RuleFor(x => x.BackgroundColor).NotEmpty().SetValidator(new ColorValidator());
            RuleFor(x => x.Font).NotEmpty().SetValidator(new FontValidator());
        }
    }
}