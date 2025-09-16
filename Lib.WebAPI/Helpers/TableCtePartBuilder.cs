using System.Text;
using System.Text.RegularExpressions;
using Lib.Athena.Consts;

namespace Lib.WebAPI.Helpers
{
    internal partial class TableCtePartBuilder : IQueryPartBuilder
    {
        [GeneratedRegex(@"(\w+_auxiliary\s*->>\s*'(impressions|impressionsactual|budgetedimpressions)')\s+as\s+([^\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
        private static partial Regex SupplierImpressionsZeroRegex();

        [GeneratedRegex(@"(.*?)\s+as\s+(mtc_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex GenericMetricPattern();

        [GeneratedRegex(@"(.*?)\s+as\s+(sblvlmtc_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex SubLevelMetricPattern();

        private static readonly string[] IndexTables = [AthenaConsts.Supplier, AthenaConsts.Placement];
        private static readonly string[] BaseColumns = ["tier_id", "parent_id"];

        public void Build(StringBuilder query, QueryParameters parameters)
        {
            for (int i = 0; i < parameters.Tables.Length; i++)
            {
                query.Append(i == 0 ? ", " : ", ");
                BuildCteForTable(query, parameters.Tables[i], parameters);
            }
        }

        private static void BuildCteForTable(StringBuilder query, string table, QueryParameters parameters)
        {
            query.Append($"{table}_data AS MATERIALIZED (SELECT ");

            // Add DISTINCT only for non-index tables for performance optimization
            if (!IndexTables.Contains(table))
            {
                query.Append("DISTINCT ");
            }

            query.Append(string.Join(", ", BaseColumns));
            var selectQuery = new StringBuilder();
            // Add table-specific columns
            if (parameters.Selection.TryGetValue(table, out var tableColumns) && tableColumns.Any())
            {
                selectQuery.Append(", ").Append(string.Join(", ", tableColumns));
            }

            // Add auxiliary columns and index if needed
            if (parameters.Selection.TryGetValue($"{table}_auxiliary", out var auxColumns) && auxColumns.Any())
            {
                selectQuery.Append(", ").Append(string.Join(", ", auxColumns));
                if (IndexTables.Contains(table))
                {
                    selectQuery.Append($", index_{table}");
                }
            }
            if (parameters.isBriefedCtc)
            {
                var briefedQuery = BuildBriefedCTCQuery(selectQuery.ToString(), parameters);
                query.Append(briefedQuery);
            }
            else
            {
                query.Append(selectQuery);
            }

            // FROM clause with auxiliary joins
            query.Append($" FROM {table}_v{parameters.PmdsVersion} AS {table}");
            if (parameters.AuxiliaryJoins.TryGetValue($"{table}_auxiliary", out var joinClause))
            {
                query.Append(", ").Append(joinClause);
            }

            // WHERE clause with EXISTS check and auxiliary conditions
            query.Append($" WHERE EXISTS (SELECT 1 FROM ids WHERE {table}_tier_id = {table}.tier_id AND {table}_parent_id = {table}.parent_id)");

            if (parameters.Where.TryGetValue($"{table}_auxiliary", out var auxWhereValues) && auxWhereValues.Any())
            {
                query.Append(" AND ").Append(string.Join(" AND ", auxWhereValues));
            }

            query.Append(')');
        }

        private static string BuildBriefedCTCQuery(string query, QueryParameters parameters)
        {
            foreach (var table in parameters.Tables.Where(x => x != AthenaConsts.Budget))
            {
                query = SupplierImpressionsZeroRegex().Replace(query, "('0') as $3");
                var regReplace = @$"\({table}_auxiliary->>'(.*?)'\)\:\:float";
                query = Regex.Replace(query, regReplace, " ((0)::float) ", RegexOptions.IgnoreCase);
                query = GenericMetricPattern().Replace(query, "0 AS $2");
                query = SubLevelMetricPattern().Replace(query, "0 AS $2");
            }
            return query;
        }
    }
}
