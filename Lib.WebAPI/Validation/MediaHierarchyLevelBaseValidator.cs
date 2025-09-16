using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyLevelBaseValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyLevelBaseValidator : AbstractValidator<MediaHierarchyLevelBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchyLevelBaseValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchyLevelBaseValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();

            RuleFor(x => x.Settings).NotEmpty();
            RuleForEach(x => x.Settings).SetValidator(new MediaHierarchySettingBaseValidator());

            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalBaseValidator());

            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}