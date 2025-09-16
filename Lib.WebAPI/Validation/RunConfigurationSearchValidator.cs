using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// RunConfigurationSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RunConfigurationSearchValidator : NamedSearchValidator<RunConfigurationSearchDTO, RunConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunConfigurationSearchValidator"
        /// /> class.
        /// </summary>
        public RunConfigurationSearchValidator()
            : base()
        {
            RuleFor(x => x.FlowchartTemplateId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}