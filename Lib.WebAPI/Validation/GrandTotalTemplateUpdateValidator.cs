using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GrandTotalTemplateUpdateDTO
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalTemplateUpdateValidator : AbstractValidator<GrandTotalTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="GrandTotalTemplateUpdateValidator" /> class.
        /// </summary>
        public GrandTotalTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new GrandTotalDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}