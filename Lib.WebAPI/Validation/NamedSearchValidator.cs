using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.Validation
{
    /// <summary>
    /// NamedSearchValidator
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class NamedSearchValidator<TNamedSearch, TEntity> : AbstractValidator<TNamedSearch>
        where TNamedSearch : NamedSearch
        where TEntity : DbModelBase
    {
        private static readonly string[] FilteredPropertyNames =
            {
                nameof(DbModelBase.Removed),
                nameof(FlowchartTemplate.FlowchartDefinition),
                nameof(CalendarTemplate.CalendarDefinition),
                nameof(MediaHierarchyTemplate.MediaHierarchyDefinition),
                nameof(HeaderTemplate.HeaderDefinition),
                nameof(FooterTemplate.FooterDefinition),
                nameof(TotalsTemplate.TotalsDefinition),
                nameof(GrandTotalTemplate.GrandTotalDefinition),
                nameof(ThemeTemplate.ThemeDefinition),
                nameof(CalendarOverlayTemplate.CalendarOverlayDefinition),
            };

        private static readonly string[] Properties = GetProperties(typeof(TEntity));

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="NamedSearchValidator{TNamedSearch, TEntity}" /> class.
        /// </summary>
        public NamedSearchValidator()
        {
            RuleFor(x => x.Count).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.OrderAscending).NotNull().When(x => x.OrderBy != null);
            RuleFor(x => x.Start).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(x => x.SearchText).NotEmpty().When(x => x.SearchText != null);
            RuleFor(x => x.OrderBy).NotEmpty().Must(x => Properties.Contains(x)).When(x => x.OrderBy != null)
                .WithMessage($"Valid options for {nameof(NamedSearch.OrderBy)}: {string.Join(", ", Properties)}.");
        }

        private static string[] GetProperties(Type type)
        {
            var result = new List<string>();

            var props = type.GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType.IsInterface || FilteredPropertyNames.Contains(prop.Name))
                {
                    continue;
                }

                if (prop.PropertyType.IsClass)
                {
                    if (prop.PropertyType.FullName?.StartsWith($"{nameof(System)}.") == false)
                    {
                        result.AddRange(GetProperties(prop.PropertyType).Select(x => $"{prop.Name}.{x}"));
                    }
                    else
                    {
                        result.Add(prop.Name);
                    }
                }
                else
                {
                    result.Add(prop.Name);
                }
            }

            return result.ToArray();
        }
    }
}