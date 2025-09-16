using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// Alignment
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AlignmentValidator : AbstractValidator<Alignment>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlignmentValidator" /> class.
        /// </summary>
        public AlignmentValidator()
        {
            // TODI
        }
    }
}