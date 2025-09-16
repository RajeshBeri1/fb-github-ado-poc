using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// ThemeDefinition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ThemeDefinitionValidator : AbstractValidator<ThemeDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeDefinitionValidator" />
        /// class.
        /// </summary>
        public ThemeDefinitionValidator()
        {
            RuleFor(x => x.CalendarTheme).NotEmpty().SetValidator(new CalendarThemeValidator());
            RuleFor(x => x.GrandTotalTheme).NotEmpty().SetValidator(new GrandTotalThemeValidator());
            RuleFor(x => x.HeaderTheme).NotEmpty().SetValidator(new HeaderThemeValidator());
            RuleFor(x => x.MediaHierarchyTheme).NotEmpty().SetValidator(new MediaHierarchyThemeValidator());
            RuleFor(x => x.Styling).NotEmpty().SetValidator(new StylingValidator());
            RuleFor(x => x.TotalsTheme).NotEmpty().SetValidator(new TotalsThemeValidator());
            RuleFor(x => x.CalendarOverlayTheme).NotEmpty().SetValidator(new CalendarOverlayThemeValidator());
            RuleFor(x => x.LegendTheme).SetValidator(new LegendThemeValidator());
        }
    }
}