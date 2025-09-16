using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FontValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FontValidator : AbstractValidator<Font>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontValidator" /> class.
        /// </summary>
        public FontValidator()
        {
            /*
            RuleFor(x => x.Color).NotEmpty().SetValidator(new ColorValidator());
            RuleFor(x => x.Family).NotEmpty();
            RuleFor(x => x.Size).NotNull();
            RuleFor(x => x.Weight).NotNull();
            */
        }
    }
}