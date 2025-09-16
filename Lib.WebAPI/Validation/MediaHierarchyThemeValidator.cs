using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyThemeValidator : AbstractValidator<MediaHierarchyTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchyThemeValidator" />
        /// class.
        /// </summary>
        public MediaHierarchyThemeValidator()
        {
            // TODO
        }
    }
}