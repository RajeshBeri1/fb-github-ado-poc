using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// MediaHierarchyDefinitionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MediaHierarchyDefinitionValidator : AbstractValidator<MediaHierarchyDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchyDefinitionValidator" /> class.
        /// </summary>
        public MediaHierarchyDefinitionValidator()
        {
            RuleFor(x => x.Levels).NotEmpty();
            RuleForEach(x => x.Levels).SetValidator(new MediaHierarchyLevelValidator());
        }
    }
}