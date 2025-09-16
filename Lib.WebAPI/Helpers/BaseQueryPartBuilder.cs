using System.Text;
using Lib.Athena.Consts;

namespace Lib.WebAPI.Helpers
{
    /// <summary>
    /// Builds the base CTE (WITH ids AS ...) section of the query.
    /// Implements Single Responsibility Principle - only handles base query construction.
    /// </summary>
    internal sealed class BaseQueryPartBuilder : IQueryPartBuilder
    {
        private const string SourceSysMandatoryField = "source_sys_mandatory";
        private static readonly string[] BaseColumns = ["tier_id", "parent_id"];
        private static readonly string[] IndexTables = [AthenaConsts.Supplier, AthenaConsts.Placement];
        public void Build(StringBuilder query, QueryParameters parameters)
        {
            query.Append("WITH ids AS MATERIALIZED (SELECT ");
            BuildBaseSelect(query, parameters);
            BuildBaseJoin(query, parameters);
            query.Append(" WHERE ");
            BuildDirectWhereClause(query, parameters);
            query.Append(" AND ");
            BuildAuxiliaryWhereClause(query, parameters);
            query.Append(')');
        }

        private static void BuildBaseSelect(StringBuilder query, QueryParameters parameters)
        {
            var selects = parameters.Tables
                .SelectMany(table => BaseColumns.Select(field => $"{table}.{field} AS {table}_{field}"));
            query.Append(string.Join(", ", selects));
        }

        private static void BuildBaseJoin(StringBuilder query, QueryParameters parameters)
        {
            for (int i = 0; i < parameters.Tables.Length; i++)
            {
                var table = parameters.Tables[i];
                if (i == 0)
                {
                    query.Append($" FROM {table}_v{parameters.PmdsVersion} AS {table}");
                }
                else
                {
                    var previousTable = parameters.Tables[i - 1];
                    query.Append($" INNER JOIN {table}_v{parameters.PmdsVersion} AS {table}")
                         .Append($" ON {table}.parent_id = {previousTable}.tier_id")
                         .Append($" AND {table}.{AthenaConsts.OmniGuid} = {previousTable}.{AthenaConsts.OmniGuid}")
                         .Append($" AND {table}.{AthenaConsts.PmdsState} = {previousTable}.{AthenaConsts.PmdsState}")
                         .Append($" AND {table}.{SourceSysMandatoryField} = {previousTable}.{SourceSysMandatoryField}");
                }
            }
        }

        private static void BuildDirectWhereClause(StringBuilder query, QueryParameters parameters)
        {
            var whereClauses = parameters.Tables.Where(c => !parameters.isBriefedCtc || !IndexTables.Any(i => i == c))
                .SelectMany(table => parameters.Where.TryGetValue(table, out var clauses) ? clauses : Enumerable.Empty<string>())
                .ToList();

            query.Append(whereClauses.Any() ? string.Join(" AND ", whereClauses) : "1=1");
        }

        private static void BuildAuxiliaryWhereClause(StringBuilder query, QueryParameters parameters)
        {
            var existsClauses = new List<string>();

            foreach (var table in parameters.Tables)
            {
                if (parameters.Where.TryGetValue($"{table}_auxiliary", out var whereValues) && whereValues.Any())
                {
                    var join = parameters.AuxiliaryJoins[$"{table}_auxiliary"];
                    existsClauses.Add($"EXISTS(SELECT 1 FROM {join} WHERE {string.Join(" AND ", whereValues)})");
                }
            }

            query.Append(existsClauses.Any() ? string.Join(" AND ", existsClauses) : "1=1");
        }
    }
}
