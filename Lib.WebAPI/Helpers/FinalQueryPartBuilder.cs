using System.Text;
using System.Text.RegularExpressions;
using Lib.Athena.Consts;

namespace Lib.WebAPI.Helpers
{
    internal partial class FinalQueryPartBuilder : IQueryPartBuilder
    {
        [GeneratedRegex(@"\(\s*budget_auxiliary\s*->>\s*(?:'briefedctc_local'|'briefedctc'|'budget_local'|'budget')\s*\)(::float\s+AS\s*.*)", RegexOptions.Compiled)]
        private static partial Regex BudgetBriefedCtcMetricZeroRegex();

        [GeneratedRegex(@"\(budget_auxiliary->>'briefedctc(_local)?'\)(::float\s+AS\s+total_briefedctc)", RegexOptions.Compiled)]
        private static partial Regex TotalBriefedCtcOrLocalZeroPatternForSelect();

        private static readonly string[] IndexTables = [AthenaConsts.Supplier, AthenaConsts.Placement];
        private const int BaseJoinsLength = 3;

        public void Build(StringBuilder query, QueryParameters parameters)
        {
            query.Append(" SELECT * FROM (SELECT ROW_NUMBER() OVER(PARTITION BY ");

            var partitionColumns = BuildPartitionColumns(parameters);
            if (parameters.isBriefedCtc)
            {
                string insertstring = $"{AthenaConsts.CampaignTable}_tier_id, {AthenaConsts.Channel}_tier_id, {AthenaConsts.Budget}_tier_id";
                query.Append(insertstring);
            }
            else
            {
                query.Append(partitionColumns);
            }
            query.Append(") AS row_num, ");
            query.Append(partitionColumns);

            query.Append(" FROM (SELECT ");
            BuildInnerSelectColumns(query, parameters);
            BuildTableJoins(query, parameters);
            query.Append(") AS budget");

            BuildIndexTableJoins(query, parameters);
            query.Append(") AS final_query WHERE row_num = 1");
            if (!parameters.isBriefedCtc)
            {
                ReplaceQueryIfBriefedSelected(query);
            }
        }

        private static void ReplaceQueryIfBriefedSelected(StringBuilder queryBuilder)
        {
            bool briefedCtcExists = queryBuilder.ToString().Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.BriefedCtc}_local')::float AS") ||
                               queryBuilder.ToString().Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.BriefedCtc}')::float AS") ||
                               queryBuilder.ToString().Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.Budget}_local')::float AS") ||
                               queryBuilder.ToString().Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.Budget}')::float AS");
            var query = queryBuilder.ToString();
            if (briefedCtcExists)
            {
                var query1 = BudgetBriefedCtcMetricZeroRegex().Replace(query, "(0)$1");
                query1 = TotalBriefedCtcOrLocalZeroPatternForSelect().Replace(query1, "(0)$2");
                queryBuilder.Clear();
                queryBuilder.Append(query1);
            }
        }

        private static string BuildPartitionColumns(QueryParameters parameters)
        {
            var columns = new List<string>();

            foreach (var tableSelect in parameters.Selection.Values)
            {
                foreach (var select in tableSelect)
                {
                    var parts = select.Split(" as ", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length > 1)
                    {
                        columns.Add(parts[1]);
                    }

                    parts = select.Split(" AS ", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length > 1)
                    {
                        columns.Add(parts[1]);
                    }
                }
            }

            // Add index table tier_ids
            foreach (var indexTable in IndexTables)
            {
                if (parameters.Selection.ContainsKey($"{indexTable}_auxiliary"))
                {
                    columns.Add($"{indexTable}.tier_id");
                }
            }

            return columns.Any() ? string.Join(", ", columns) : "1"; // Fallback to prevent empty partition
        }

        private static void BuildInnerSelectColumns(StringBuilder query, QueryParameters parameters)
        {
            var nonIndexTables = parameters.Tables.Where(table => !IndexTables.Contains(table)).ToArray();
            var columns = new List<string>();

            foreach (var tableSelect in parameters.Selection.Where(kvp =>
                nonIndexTables.Contains(kvp.Key) || nonIndexTables.Any(t => $"{t}_auxiliary" == kvp.Key)))
            {
                foreach (var select in tableSelect.Value)
                {
                    var parts = select.Split(" as ", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length > 1)
                    {
                        columns.Add(parts[1]);
                    }

                    parts = select.Split(" AS ", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length > 1)
                    {
                        columns.Add(parts[1]);
                    }
                }
            }
            if (!parameters.isBriefedCtc)
            {
                // Add the final table's tier_id for joins
                if (nonIndexTables.Any())
                {
                    var finalTable = nonIndexTables.Last();
                    columns.Add($"{finalTable}.tier_id");
                }
            }
            else
            {
                // Add the final table's tier_id for joins
                foreach (var table in nonIndexTables)
                {
                    columns.Add($"{table}.tier_id AS {table}_tier_id");
                }
            }

            query.Append(columns.Any() ? string.Join(", ", columns) : "*");
        }

        private static void BuildTableJoins(StringBuilder query, QueryParameters parameters)
        {
            var nonIndexTables = parameters.Tables.Where(table => !IndexTables.Contains(table)).ToArray();

            for (int i = 0; i < nonIndexTables.Length; i++)
            {
                var table = nonIndexTables[i];
                if (i == 0)
                {
                    query.Append($" FROM {table}_data AS {table}");
                }
                else
                {
                    var previousTable = nonIndexTables[i - 1];
                    bool needsJoin = ShouldIncludeJoin(table, parameters);

                    if (needsJoin)
                    {
                        query.Append($" INNER JOIN {table}_data AS {table} ON {table}.parent_id = {previousTable}.tier_id");
                    }
                }
            }
        }

        private static void BuildIndexTableJoins(StringBuilder query, QueryParameters parameters)
        {
            foreach (var table in parameters.Tables.Where(IndexTables.Contains))
            {
                bool needsJoin = ShouldIncludeJoin(table, parameters);

                if (needsJoin)
                {
                    var previousTable = GetPreviousTable(table, parameters.Tables);
                    if (parameters.isBriefedCtc && previousTable == AthenaConsts.Budget)
                    {
                        query.Append($" INNER JOIN {table}_data AS {table} ON {table}.parent_id = {previousTable}.{previousTable}_tier_id");
                    }
                    else
                    {
                        query.Append($" INNER JOIN {table}_data AS {table} ON {table}.parent_id = {previousTable}.tier_id");

                    }
                }
            }
        }

        private static bool ShouldIncludeJoin(string table, QueryParameters parameters)
        {
            return table != AthenaConsts.Placement ||
                   parameters.Selection.ContainsKey(table) ||
                   parameters.Selection.ContainsKey($"{table}_auxiliary") ||
                   (parameters.Where.TryGetValue(table, out var whereClauses) && whereClauses.Count > BaseJoinsLength) ||
                   parameters.Where.ContainsKey($"{table}_auxiliary");
        }

        private static string GetPreviousTable(string currentTable, string[] tables)
        {
            var index = Array.IndexOf(tables, currentTable);
            return index > 0 ? tables[index - 1] : AthenaConsts.Budget;
        }
    }
}
