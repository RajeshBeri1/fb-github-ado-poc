using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// SummaryTemplateSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SummaryTemplateSearchValidator : NamedSearchValidator<SummaryTemplateSearchDTO, SummaryTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryTemplateSearchValidator"
        /// /> class.
        /// </summary>
        public SummaryTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}