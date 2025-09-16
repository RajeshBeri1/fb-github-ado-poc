using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyTemplateSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyTemplateSearchValidator : NamedSearchValidator<MediaHierarchyTemplateSearchDTO, MediaHierarchyTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchyTemplateSearchValidator" /> class.
        /// </summary>
        public MediaHierarchyTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}