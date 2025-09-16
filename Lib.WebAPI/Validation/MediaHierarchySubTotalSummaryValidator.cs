using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubTotalSummaryValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalSummaryValidator : AbstractValidator<MediaHierarchySubTotalSummary>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchySubTotalSummaryValidator" />
        /// class.
        /// </summary>
        public MediaHierarchySubTotalSummaryValidator()
        {
            RuleFor(x => x.Settings).NotEmpty();
            RuleFor(x => x.SubTotals).NotEmpty();

            RuleForEach(x => x.Settings).SetValidator(new MediaHierarchySubTotalSummarySettingValidator());

            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalBaseValidator());
        }
    }
}