using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// FlowchartDefinitionValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FlowchartDefinitionValidator : AbstractValidator<FlowchartDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartDefinitionValidator" />
        /// class.
        /// </summary>
        public FlowchartDefinitionValidator()
        {
            RuleFor(x => x.CalendarDefinition!).SetValidator(new DefinitionReferenceValidator<CalendarDefinition>(new CalendarDefinitionValidator()));
            RuleFor(x => x.CalendarOverlayDefinition!).SetValidator(new DefinitionReferenceValidator<CalendarOverlayDefinition>(new CalendarOverlayDefinitionValidator()));
            RuleFor(x => x.GrandTotalDefinition!).SetValidator(new DefinitionReferenceValidator<GrandTotalDefinition>(new GrandTotalDefinitionValidator()));
            RuleFor(x => x.HeaderDefinition!).SetValidator(new DefinitionReferenceValidator<HeaderDefinition>(new HeaderDefinitionValidator()));
            RuleFor(x => x.FooterDefinition!).SetValidator(new DefinitionReferenceValidator<FooterDefinition>(new FooterDefinitionValidator()));
            RuleFor(x => x.MediaHierarchyDefinition!).SetValidator(new DefinitionReferenceValidator<MediaHierarchyDefinition>(new MediaHierarchyDefinitionValidator()));
            RuleFor(x => x.ThemeDefinition!).SetValidator(new DefinitionReferenceValidator<ThemeDefinition>(new ThemeDefinitionValidator()));
            RuleFor(x => x.TotalsDefinition!).SetValidator(new DefinitionReferenceValidator<TotalsDefinition>(new TotalsDefinitionValidator()));
            RuleFor(x => x.SummaryDefinition!).SetValidator(new DefinitionReferenceValidator<SummaryDefinition>(new SummaryDefinitionValidator()));
        }
    }
}