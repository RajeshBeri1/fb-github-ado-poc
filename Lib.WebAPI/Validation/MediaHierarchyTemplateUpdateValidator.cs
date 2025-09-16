using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyTemplateUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyTemplateUpdateValidator : AbstractValidator<MediaHierarchyTemplateUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchyTemplateUpdateValidator" /> class.
        /// </summary>
        public MediaHierarchyTemplateUpdateValidator()
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(new MediaHierarchyDefinitionValidator());
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty();
        }
    }
}