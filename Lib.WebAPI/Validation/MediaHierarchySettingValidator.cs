using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySettingValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySettingValidator : AbstractValidator<MediaHierarchySetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchySettingValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchySettingValidator()
        {
            // RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
            RuleFor(x => x.Enabled).NotNull();
            RuleFor(x => x.FlightRange).NotNull();
            //RuleFor(x => x.MetricColumnName).NotEmpty();
            //RuleFor(x => x.MetricTableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleForEach(x => x.SubLevels).SetValidator(new MediaHierarchySubLevelValidator());
            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalValidator());
            RuleForEach(x => x.SubTotalSummary).SetValidator(new MediaHierarchySubTotalSummaryValidator());
        }
    }
}