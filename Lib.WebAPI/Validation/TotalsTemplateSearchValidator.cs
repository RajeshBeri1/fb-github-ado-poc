using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// TotalsTemplateSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TotalsTemplateSearchValidator : NamedSearchValidator<TotalsTemplateSearchDTO, TotalsTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TotalsTemplateSearchValidator" />
        /// class.
        /// </summary>
        public TotalsTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}