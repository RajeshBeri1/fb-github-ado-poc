using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyTemplateCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyTemplateCreateValidator : AbstractValidator<MediaHierarchyTemplateCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchyTemplateCreateValidator" /> class.
        /// </summary>
        public MediaHierarchyTemplateCreateValidator()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new MediaHierarchyDefinitionValidator());
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty();
        }
    }
}