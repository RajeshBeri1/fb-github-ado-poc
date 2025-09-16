using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyLevelValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyLevelValidator : AbstractValidator<MediaHierarchyLevel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchyLevelValidator" />
        /// class.
        /// </summary>
        public MediaHierarchyLevelValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();

            RuleFor(x => x.Settings).NotEmpty();
            RuleForEach(x => x.Settings).SetValidator(new MediaHierarchySettingValidator());

            RuleForEach(x => x.SubTotals).SetValidator(new MediaHierarchySubTotalValidator());

            RuleForEach(x => x.InflightOverlays).SetValidator(new MediaHierarchyInflightOverlayValidator());

            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}