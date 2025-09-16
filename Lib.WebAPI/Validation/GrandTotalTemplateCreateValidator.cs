using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// GrandTotalTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GrandTotalTemplateCreateValidator : AbstractValidator<GrandTotalTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="GrandTotalTemplateCreateValidator" /> class.
        /// </summary>
        public GrandTotalTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new GrandTotalDefinitionValidator());
        }
    }
}