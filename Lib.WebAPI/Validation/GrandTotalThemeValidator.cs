using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GrandTotalTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalThemeValidator : AbstractValidator<GrandTotalTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrandTotalThemeValidator" />
        /// class.
        /// </summary>
        public GrandTotalThemeValidator()
        {
            // TODO
        }
    }
}