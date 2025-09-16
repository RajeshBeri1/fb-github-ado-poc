using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubLevelBaseValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubLevelBaseValidator : AbstractValidator<MediaHierarchySubLevelBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchySubLevelBaseValidator" /> class.
        /// </summary>
        public MediaHierarchySubLevelBaseValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();

            RuleFor(x => x.Settings).NotEmpty();
            RuleForEach(x => x.Settings).SetValidator(new MediaHierarchySubLevelSettingBaseValidator());

            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalBaseValidator());

            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}