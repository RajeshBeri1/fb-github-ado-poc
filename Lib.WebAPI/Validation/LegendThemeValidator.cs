using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.Models.Flowchart;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// LegendTheme
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LegendThemeValidator : AbstractValidator<LegendTheme>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LegendThemeValidator" /> class.
        /// </summary>
        public LegendThemeValidator()
        {
            //RuleFor(x => x.TableId).NotEmpty().When(x =>string.IsNullOrWhiteSpace(x.ColumnName));

           // RuleFor(x => x.ColumnName).NotEmpty().When(x => x.TableId != null);

            RuleForEach(x => x.Settings).SetValidator(new LegendThemeSettingValidator());
        }
    }
}