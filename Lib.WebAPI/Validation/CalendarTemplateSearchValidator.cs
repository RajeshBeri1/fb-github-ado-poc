using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// CalendarTemplateSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CalendarTemplateSearchValidator : NamedSearchValidator<CalendarTemplateSearchDTO, CalendarTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarTemplateSearchValidator"
        /// /> class.
        /// </summary>
        public CalendarTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}