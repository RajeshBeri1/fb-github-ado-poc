using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubLevelValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubLevelValidator : AbstractValidator<MediaHierarchySubLevel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchySubLevelValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchySubLevelValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();

            RuleFor(x => x.Settings).NotEmpty();
            RuleForEach(x => x.Settings).SetValidator(new MediaHierarchySubLevelSettingValidator());

            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalValidator());

            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}