using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// ThemeTemplateSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeTemplateSearchValidator : NamedSearchValidator<ThemeTemplateSearchDTO, ThemeTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeTemplateSearchValidator" />
        /// class.
        /// </summary>
        public ThemeTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}