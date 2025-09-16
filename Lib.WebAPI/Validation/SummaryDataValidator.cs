using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Data;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryDataValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryDataValidator : AbstractValidator<SummaryData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryDataValidator" /> class.
        /// </summary>
        public SummaryDataValidator()
        {
            // TODO
        }
    }
}