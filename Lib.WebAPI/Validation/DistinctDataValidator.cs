using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// DistinctDataValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DistinctDataValidator : AbstractValidator<DistinctDataDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistinctDataValidator" /> class.
        /// </summary>
        public DistinctDataValidator()
        {
            RuleFor(x => x.TableId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.ColumnName).NotEmpty();
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}