using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySettingStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySettingStylingValidator : AbstractValidator<MediaHierarchySettingStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchySettingStylingValidator" /> class.
        /// </summary>
        public MediaHierarchySettingStylingValidator()
        {
            RuleFor(x => x.Alignment).NotNull();
            RuleFor(x => x.BackgroundColor).NotEmpty().SetValidator(new ColorValidator());
            RuleFor(x => x.Font).NotEmpty().SetValidator(new FontValidator());
        }
    }
}