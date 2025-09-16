using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderDetails
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDetailsValidator : AbstractValidator<HeaderDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDetailsValidator" /> class.
        /// </summary>
        public HeaderDetailsValidator()
        {
            RuleFor(x => x.Rows).NotEmpty();
            RuleForEach(x => x.Rows).SetValidator(new HeaderDetailsRowValidator());
        }
    }
}