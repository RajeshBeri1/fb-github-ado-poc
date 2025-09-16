using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubTotalValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalValidator : AbstractValidator<MediaHierarchySubTotal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHierarchySubTotalValidator"
        /// /> class.
        /// </summary>
        public MediaHierarchySubTotalValidator()
        {
            RuleFor(x => x.FlightRange).NotNull();
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Order).NotNull();
            // RuleFor(x => x.Styling).NotEmpty().SetValidator(new MediaHierarchySubTotalStylingValidator());
        }
    }
}