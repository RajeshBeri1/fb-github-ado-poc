using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// RunConfigurationUpdateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationUpdateValidator : AbstractValidator<RunConfigurationUpdateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunConfigurationUpdateValidator"
        /// /> class.
        /// </summary>
        public RunConfigurationUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.MediaHierarchyLevels).NotEmpty();
            RuleFor(x => x.RunRestrictions).NotNull();

            RuleForEach(x => x.MediaHierarchyLevels).SetValidator(new MediaHierarchyLevelBaseValidator());
            RuleForEach(x => x.RunRestrictions).SetValidator(new RunRestrictionValidator());
        }
    }
}