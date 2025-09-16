using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchySubTotalBaseValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchySubTotalBaseValidator : AbstractValidator<MediaHierarchySubTotalBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchySubTotalBaseValidator" /> class.
        /// </summary>
        public MediaHierarchySubTotalBaseValidator()
        {
            RuleFor(x => x.FlightRange).NotNull();
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Order).NotNull();
        }
    }
}