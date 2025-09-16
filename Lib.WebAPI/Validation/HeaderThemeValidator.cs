using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderThemeValidator : AbstractValidator<HeaderTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderThemeValidator" /> class.
        /// </summary>
        public HeaderThemeValidator()
        {
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());

            // TODO

            // RuleFor(x => x.RowStyling).NotEmpty();
        }
    }
}