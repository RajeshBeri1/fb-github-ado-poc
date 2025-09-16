using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderLogo
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderLogoValidator : AbstractValidator<HeaderLogo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderLogoValidator" /> class.
        /// </summary>
        public HeaderLogoValidator()
        {
            RuleFor(x => x.Image).NotEmpty().SetValidator(new ImageValidator());
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.Alignment).NotNull();
        }
    }
}