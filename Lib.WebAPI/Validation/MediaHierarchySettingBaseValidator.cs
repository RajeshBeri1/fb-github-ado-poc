using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySettingBaseValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySettingBaseValidator : AbstractValidator<MediaHierarchySettingBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchySettingBaseValidator" /> class.
        /// </summary>
        public MediaHierarchySettingBaseValidator()
        {
            RuleFor(x => x.Enabled).NotNull();
            RuleFor(x => x.FlightRange).NotNull();
            RuleFor(x => x.MetricColumnName).NotEmpty();
            RuleFor(x => x.MetricTableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleForEach(x => x.SubLevels).SetValidator(new MediaHierarchySubLevelBaseValidator());
            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalBaseValidator());
        }
    }
}