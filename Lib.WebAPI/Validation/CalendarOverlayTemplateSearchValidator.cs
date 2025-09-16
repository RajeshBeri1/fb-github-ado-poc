using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarOverlayTemplateSearchDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarOverlayTemplateSearchValidator : NamedSearchValidator<CalendarOverlayTemplateSearchDTO, CalendarOverlayTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlayTemplateSearchValidator" /> class.
        /// </summary>
        public CalendarOverlayTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}