using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubTotalStyling
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalStylingValidator : AbstractValidator<MediaHierarchySubTotalStyling>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchySubTotalStylingValidator" /> class.
        /// </summary>
        public MediaHierarchySubTotalStylingValidator()
        {
            RuleFor(x => x.Alignment).NotNull();
            RuleFor(x => x.Font).NotEmpty().SetValidator(new FontValidator());
        }
    }
}