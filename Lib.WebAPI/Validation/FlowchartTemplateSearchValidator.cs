using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FlowchartTemplateSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplateSearchValidator : NamedSearchValidator<FlowchartTemplateSearchDTO, FlowchartTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplateSearchValidator"
        /// /> class.
        /// </summary>
        public FlowchartTemplateSearchValidator()
            : base()
        {
            RuleFor(x => x.OmniClientId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}