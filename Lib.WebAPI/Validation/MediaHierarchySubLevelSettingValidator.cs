using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubLevelSetting
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubLevelSettingValidator : AbstractValidator<MediaHierarchySubLevelSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchySubLevelSettingValidator" /> class.
        /// </summary>
        public MediaHierarchySubLevelSettingValidator()
        {
            // RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
            RuleFor(x => x.Enabled).NotNull();
            RuleFor(x => x.FlightRange).NotNull();
            //RuleFor(x => x.MetricColumnName).NotEmpty();
            //RuleFor(x => x.MetricTableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalValidator());
        }
    }
}