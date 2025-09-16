using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// LegendThemeSetting
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LegendThemeSettingValidator : AbstractValidator<LegendThemeSetting>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LegendThemeSettingValidator" /> class.
        /// </summary>
        public LegendThemeSettingValidator()
        {
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
            RuleFor(x => x.InflightOverlayStyling).SetValidator(new StylingValidator()).When(c=>c.InflightOverlayStyling!=null);
            RuleFor(x => x.SubTotalStyling).SetValidator(new StylingValidator()).When(c => c.SubTotalStyling != null);

            RuleFor(x => x.Name).NotEmpty();
        }
    }
}