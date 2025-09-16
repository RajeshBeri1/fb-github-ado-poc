using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// TotalsTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsThemeValidator : AbstractValidator<TotalsTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TotalsThemeValidator" /> class.
        /// </summary>
        public TotalsThemeValidator()
        {
            // TODO
        }
    }
}