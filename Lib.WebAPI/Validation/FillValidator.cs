using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// Fill
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FillValidator : AbstractValidator<Fill>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FillValidator" /> class.
        /// </summary>
        public FillValidator()
        {
            // TODO
        }
    }
}