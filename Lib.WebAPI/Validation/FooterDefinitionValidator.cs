using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FooterDefinitionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FooterDefinitionValidator : AbstractValidator<FooterDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooterDefinitionValidator" />
        /// class.
        /// </summary>
        public FooterDefinitionValidator()
        {
            RuleFor(x => x.Configuration).NotEmpty().SetValidator(new FooterConfigurationValidator());
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
        }
    }
}