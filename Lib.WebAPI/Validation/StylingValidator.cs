using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// Styling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StylingValidator : AbstractValidator<Styling>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StylingValidator" /> class.
        /// </summary>
        public StylingValidator()
        {
            RuleFor(x => x.Alignment).NotEmpty().SetValidator(new AlignmentValidator());
            RuleFor(x => x.Border).NotEmpty().SetValidator(new BorderValidator());
            RuleFor(x => x.Fill).NotEmpty().SetValidator(new FillValidator());
            RuleFor(x => x.Font).NotEmpty().SetValidator(new FontValidator());
        }
    }
}