using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// ReportPublishValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReportPublishValidator : AbstractValidator<ReportPublishDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportPublishValidator" /> class.
        /// </summary>
        public ReportPublishValidator()
        {
            RuleFor(x => x.Data).NotEmpty().SetValidator(new DataValidator());
            RuleFor(x => x.FileContent).NotEmpty();
            RuleFor(x => x.FileType).NotNull().Equals(FileType.Excel);
            RuleFor(x => x.RunConfigurationId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RunConfigurationData).NotEmpty().SetValidator(new RunConfigurationDataValidator());
            RuleFor(x => x.Comments).NotEmpty();
        }
    }
}