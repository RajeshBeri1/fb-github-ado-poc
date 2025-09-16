using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// LoginValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginValidator" /> class.
        /// </summary>
        public LoginValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.UserName).NotEmpty();
        }
    }
}