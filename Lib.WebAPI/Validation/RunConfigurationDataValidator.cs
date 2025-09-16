using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Data;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// RunConfigurationDataValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationDataValidator : AbstractValidator<RunConfigurationData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunConfigurationDataValidator" />
        /// class.
        /// </summary>
        public RunConfigurationDataValidator()
        {
            RuleFor(x => x.MediaHierarchyLevels).NotEmpty();
            RuleForEach(x => x.MediaHierarchyLevels).SetValidator(new MediaHierarchyLevelBaseValidator());
            RuleFor(x => x.RunRestrictions).NotNull();
            RuleForEach(x => x.RunRestrictions).SetValidator(new RunRestrictionValidator());
        }
    }
}