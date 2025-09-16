using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GrandTotalTemplateSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalTemplateSearchValidator : NamedSearchValidator<GrandTotalTemplateSearchDTO, GrandTotalTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="GrandTotalTemplateSearchValidator" /> class.
        /// </summary>
        public GrandTotalTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}