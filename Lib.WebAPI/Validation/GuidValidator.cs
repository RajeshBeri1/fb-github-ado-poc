using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GuidValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GuidValidator : AbstractValidator<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidValidator" /> class.
        /// </summary>
        public GuidValidator()
        {
            RuleFor(x => x).NotEmpty().NotEqual(Guid.Empty).WithName("Id");
        }
    }
}