using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// Border
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BorderValidator : AbstractValidator<Border>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderValidator" /> class.
        /// </summary>
        public BorderValidator()
        {
            // TODO
        }
    }
}