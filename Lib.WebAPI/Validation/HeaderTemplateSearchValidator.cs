using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderTemplateSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderTemplateSearchValidator : NamedSearchValidator<HeaderTemplateSearchDTO, HeaderTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderTemplateSearchValidator" />
        /// class.
        /// </summary>
        public HeaderTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}