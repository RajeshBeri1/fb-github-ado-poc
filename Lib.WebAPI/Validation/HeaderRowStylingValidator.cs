using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderRowStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderRowStylingValidator : AbstractValidator<HeaderRowStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderRowStylingValidator" />
        /// class.
        /// </summary>
        public HeaderRowStylingValidator()
        {
            /*
            RuleFor(x => x.Alignment).NotNull();
            RuleFor(x => x.BackgroundColor).NotEmpty().SetValidator(new ColorValidator());
            RuleFor(x => x.Font).NotEmpty().SetValidator(new FontValidator());
            */
        }
    }
}