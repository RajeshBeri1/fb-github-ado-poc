using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyInflightOverlayValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyInflightOverlayValidator : AbstractValidator<MediaHierarchyInflightOverlay>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchyInflightOverlayValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchyInflightOverlayValidator()
        {
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            // RuleFor(x => x.Styling).NotEmpty().SetValidator(new MediaHierarchyInflightOverlayStylingValidator());
        }
    }
}