using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubTotalSummarySettingValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalSummarySettingValidator : AbstractValidator<MediaHierarchySubTotalSummarySettingBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchySubTotalSummarySettingValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchySubTotalSummarySettingValidator()
        {
            RuleFor(x => x.Values).NotNull().NotEmpty();
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Order).NotNull();
        }
    }
}