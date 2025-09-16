using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Data;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderDataValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDataValidator : AbstractValidator<HeaderData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDataValidator" /> class.
        /// </summary>
        public HeaderDataValidator()
        {
            // TODO
        }
    }
}