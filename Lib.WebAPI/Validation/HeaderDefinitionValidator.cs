using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// HeaderDefinitionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HeaderDefinitionValidator : AbstractValidator<HeaderDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDefinitionValidator" />
        /// class.
        /// </summary>
        public HeaderDefinitionValidator()
        {
            RuleFor(x => x.Configuration).NotEmpty().SetValidator(new HeaderConfigurationValidator());
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
        }
    }
}