using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Lib.Athena.Consts;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using Lib.WebAPI.Extensions;

namespace Lib.WebAPI.Helpers
{
    #region Core Contracts and Models

    /// <summary>
    /// Encapsulates all parameters required for building SQL queries.
    /// Follows the Parameter Object pattern to reduce method signatures complexity.
    /// </summary>

    public record QueryParameters(
        string[] Tables,
        int PmdsVersion,
        IReadOnlyDictionary<string, string> AuxiliaryJoins,
        IReadOnlyDictionary<string, List<string>> Selection,
        IReadOnlyDictionary<string, List<string>> Where,
        bool isBriefedCtc = false)
    {
        public static QueryParameters Create(
            string[] tables,
            int pmdsVersion,
            Dictionary<string, string> auxiliaryJoins,
            Dictionary<string, List<string>> selection,
            Dictionary<string, List<string>> where,
            bool isBriefedCtc)
        {
            return new QueryParameters(
                tables,
                pmdsVersion,
                auxiliaryJoins.AsReadOnly(),
                selection,
                where,
                isBriefedCtc);
        }
    }

    /// <summary>
    /// Main query builder that orchestrates the construction of complex SQL queries.
    /// Implements Builder pattern with Strategy pattern for individual query parts.
    /// Follows Open/Closed Principle - extensible through new strategies without modification.
    /// </summary>
    public sealed class QueryBuilder
    {
        private readonly IReadOnlyList<IQueryPartBuilder> _partBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// Initializes a new instance of QueryBuilder with custom part builders.
        /// Enables Dependency Inversion - depends on abstractions, not concretions.
        /// </summary>
        /// <param name="partBuilders">Ordered collection of query part builders.</param>
        public QueryBuilder(IEnumerable<IQueryPartBuilder> partBuilders)
        {
            _partBuilders = partBuilders?.ToArray() ?? throw new ArgumentNullException(nameof(partBuilders));
        }

        /// <summary>
        /// Builds the complete SQL query using the configured strategies.
        /// </summary>
        /// <param name="parameters">Query construction parameters.</param>
        /// <returns>Complete SQL query string.</returns>
        public string Build(QueryParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var query = new StringBuilder();
            foreach (var builder in _partBuilders)
            {
                builder.Build(query, parameters);
            }
            return query.ToString();
        }

        /// <summary>
        /// Factory method that creates a QueryBuilder with default strategies.
        /// Provides a convenient way to use the builder without manual configuration.
        /// </summary>
        /// <returns>QueryBuilder configured with standard query part builders.</returns>
        public static QueryBuilder CreateDefault()
        {
            return new QueryBuilder(
            [
                new BaseQueryPartBuilder(),
                new TableCtePartBuilder(),
                new FinalQueryPartBuilder()
            ]);
        }

        /// <summary>
        /// Static convenience method that maintains backward compatibility.
        /// Delegates to the new builder pattern while preserving the original API.
        /// </summary>
        /// <param name="tables">Array of table names to include in the query.</param>
        /// <param name="pmdsVersion">PMDS version number for table naming.</param>
        /// <param name="auxiliaryJoins">Dictionary of auxiliary join clauses.</param>
        /// <param name="selection">Dictionary of column selections by table.</param>
        /// <param name="where">Dictionary of WHERE clause conditions by table.</param>
        /// <param name="isBriefedCtc">Dictionary of isBriefedCtc clause conditions by table.</param>
        /// <returns>Complete SQL query string.</returns>
        public static string Build(
            string[] tables,
            int pmdsVersion,
            Dictionary<string, string> auxiliaryJoins,
            Dictionary<string, List<string>> selection,
            Dictionary<string, List<string>> where,
            bool isBriefedCtc = false)
        {
            var parameters = QueryParameters.Create(tables, pmdsVersion, auxiliaryJoins, selection, where, isBriefedCtc);
            var builder = CreateDefault();
            return builder.Build(parameters);
        }
    }

    #endregion
}