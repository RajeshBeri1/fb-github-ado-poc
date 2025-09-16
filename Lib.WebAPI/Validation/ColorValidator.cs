using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// Color
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ColorValidator : AbstractValidator<Color>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorValidator" /> class.
        /// </summary>
        public ColorValidator()
        {
            RuleFor(x => x.Alpha).InclusiveBetween(0, 255).WithMessage("Alpha value must be between 0 and 255.");
            RuleFor(x => x.Red).InclusiveBetween(0, 255).WithMessage("Red value must be between 0 and 255.");
            RuleFor(x => x.Green).InclusiveBetween(0, 255).WithMessage("Green value must be between 0 and 255.");
            RuleFor(x => x.Blue).InclusiveBetween(0, 255).WithMessage("Blue value must be between 0 and 255.");
        }
    }
}