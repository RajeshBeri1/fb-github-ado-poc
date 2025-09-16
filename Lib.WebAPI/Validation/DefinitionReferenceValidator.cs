using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// DefinitionReferenceValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefinitionReferenceValidator<T> : AbstractValidator<DefinitionReference<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionReferenceValidator{T}"
        /// /> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public DefinitionReferenceValidator(AbstractValidator<T> validator)
        {
            RuleFor(x => x.Definition).NotEmpty().SetValidator(validator);
            RuleFor(x => x.TemplateId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.TemplateVersion).NotNull();
        }
    }
}