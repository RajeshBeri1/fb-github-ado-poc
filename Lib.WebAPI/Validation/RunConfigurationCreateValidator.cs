using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// RunConfigurationCreateValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationCreateValidator : AbstractValidator<RunConfigurationCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunConfigurationCreateValidator"
        /// /> class.
        /// </summary>
        public RunConfigurationCreateValidator()
        {
            RuleFor(x => x.FlowchartTemplateId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.MediaHierarchyLevels).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RunRestrictions).NotNull();

            RuleForEach(x => x.MediaHierarchyLevels).SetValidator(new MediaHierarchyLevelBaseValidator());
            RuleForEach(x => x.RunRestrictions).SetValidator(new RunRestrictionValidator());
            RuleFor(x => x.Comments).NotEmpty();
        }
    }
}