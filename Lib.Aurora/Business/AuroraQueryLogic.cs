using Amazon.Athena;
using Amazon.Athena.Model;
using Lib.Athena.Models;
using Lib.Aurora.Business.Interfaces;
using Lib.Aurora.Consts;
using Lib.Aurora.Models;
using Lib.Common.Business.Interfaces;
using Lib.Common.Models;
using Lib.MediaopsToFlowChart.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Globalization;
using Column = Lib.MediaopsToFlowChart.Models.Column;

namespace Lib.Aurora.Business
{
    /// <summary>
    /// AthenaQueryLogic
    /// </summary>
    public class AuroraQueryLogic : IAuroraQueryLogic
    {

        private readonly ILog<IAuroraQueryLogic> log;
        private readonly IOptionsMonitor<ConnectionStrings> connectionStrings;
        /// <summary>
        /// The table media plan join
        /// </summary>
        public static readonly string TableJoinMediaPlans = $@"FROM {AthenaConsts.CampaignTable}           
            INNER JOIN {AthenaConsts.MediaPlansTable} ON
            (
                {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.PlanId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.AmdState}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.DataState}
            )";

        /// <summary>
        /// The table media brief join
        /// </summary>
        public static readonly string TableJoinMediaBriefs = $@"FROM {AthenaConsts.CampaignTable}           
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON
            (
                {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState}
            )";

        /// <summary>
        /// Initializes a new instance of the <see cref="AuroraQueryLogic" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="connectionStrings">The connectionStrings.</param>
        public AuroraQueryLogic(ILog<IAuroraQueryLogic> log, IOptionsMonitor<ConnectionStrings> connectionStrings)
        {
            this.log = log;
            this.connectionStrings = connectionStrings;
        }

        /// <summary>
        /// Gets the table column infos asynchronous.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<ColumnInfo>> GetTableColumnInfosAsync(string table, CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM {table} LIMIT 1";
            var result = await QueryAsync(query, cancellationToken);
            return result.ColumnInfo;
        }

        /// <summary>
        /// Gets the table column infos asynchronous.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<Row>> GetDisplayNameFromClientColumnAliasesTable(Guid omniGuid, CancellationToken cancellationToken)
        {
            var rows = new List<Row>();
            var query = $@"
        SELECT DISTINCT {AthenaConsts.SourceColumnName} AS {AthenaConsts.AliasesColumnName}, {AthenaConsts.AliasesColumnAlias}
        FROM {AthenaConsts.PlannedMedia}.{AthenaConsts.OmniguidColumnAliases}
        WHERE UPPER({AthenaConsts.OmniGuid}) = @OmniGuid";

            try
            {
                await using var connection = new NpgsqlConnection(connectionStrings.CurrentValue.AuroraDB);
                await connection.OpenAsync(cancellationToken);

                await using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@OmniGuid", omniGuid.ToString().ToUpperInvariant());

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    var row = new Row
                    {
                        Data = new List<Datum>
                {
                    new Datum { VarCharValue = reader.IsDBNull(0) ? null : reader.GetString(0) },
                    new Datum { VarCharValue = reader.IsDBNull(1) ? null : reader.GetString(1) }
                }
                    };
                    rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, "Error in GetDisplayNameFromClientColumnAliasesTable.", ex);
                // Optionally, rethrow or return an empty list
                throw;
            }

            return rows;
        }



        /// <summary>
        /// Gets the client column details using clientId and omniGuid asynchronous.
        /// </summary>
        /// <param name="clientId">The table.</param>
        /// <param name="omniGuid">The omniGuid</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<TemplatesClientColumnAliases>> GetClientColumnDetailsByClientIdAndOmniGuid(
            string clientId,
            Guid omniGuid,
            CancellationToken cancellationToken)
        {
            var query = @"
    SELECT template_tracking_id, omniguid, client_name, client_id, source_sys, pmds_column_name, source_column_name, columnalias, tier, data_type, designation, template_uid, affiliated_uid, is_part_of_tier_id, is_a_group_by_column, is_part_of_display_name, is_client_default, last_refreshed_at
    FROM planned_media.mv_templates_client_column_aliases
    WHERE UPPER(omniguid) = @OmniGuid
      AND UPPER(client_name) = @ClientName";

            var items = new List<TemplatesClientColumnAliases>();

            await using (var connection = new NpgsqlConnection(connectionStrings.CurrentValue.AuroraDB))
            {
                await connection.OpenAsync(cancellationToken);
                    try
                    {
                        await using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@OmniGuid", omniGuid.ToString().ToUpperInvariant());
                            command.Parameters.AddWithValue("@ClientName", clientId.ToUpperInvariant());
                            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                            {
                                while (await reader.ReadAsync(cancellationToken))
                                {
                                    items.Add(new TemplatesClientColumnAliases
                                    {
                                        template_tracking_id = reader.GetInt32(reader.GetOrdinal("template_tracking_id")),
                                        omniguid = reader.IsDBNull(reader.GetOrdinal("omniguid")) ? null : reader.GetString(reader.GetOrdinal("omniguid")),
                                        client_name = reader.IsDBNull(reader.GetOrdinal("client_name")) ? null : reader.GetString(reader.GetOrdinal("client_name")),
                                        client_id = reader.IsDBNull(reader.GetOrdinal("client_id")) ? null : reader.GetString(reader.GetOrdinal("client_id")),
                                        source_sys = reader.IsDBNull(reader.GetOrdinal("source_sys")) ? null : reader.GetString(reader.GetOrdinal("source_sys")),
                                        pmds_column_name = reader.IsDBNull(reader.GetOrdinal("pmds_column_name")) ? null : reader.GetString(reader.GetOrdinal("pmds_column_name")),
                                        source_column_name = reader.IsDBNull(reader.GetOrdinal("source_column_name")) ? null : reader.GetString(reader.GetOrdinal("source_column_name")),
                                        columnalias = reader.IsDBNull(reader.GetOrdinal("columnalias")) ? null : reader.GetString(reader.GetOrdinal("columnalias")),
                                        tier = reader.IsDBNull(reader.GetOrdinal("tier")) ? null : reader.GetString(reader.GetOrdinal("tier")),
                                        data_type = reader.IsDBNull(reader.GetOrdinal("data_type")) ? null : reader.GetString(reader.GetOrdinal("data_type")),
                                        designation = reader.IsDBNull(reader.GetOrdinal("designation")) ? null : reader.GetString(reader.GetOrdinal("designation")),
                                        template_uid = reader.IsDBNull(reader.GetOrdinal("template_uid")) ? null : reader.GetString(reader.GetOrdinal("template_uid")),
                                        affiliated_uid = reader.IsDBNull(reader.GetOrdinal("affiliated_uid")) ? null : reader.GetString(reader.GetOrdinal("affiliated_uid")),
                                        is_part_of_tier_id = reader.GetBoolean(reader.GetOrdinal("is_part_of_tier_id")),
                                        is_a_group_by_column = reader.GetBoolean(reader.GetOrdinal("is_a_group_by_column")),
                                        is_part_of_display_name = reader.GetBoolean(reader.GetOrdinal("is_part_of_display_name")),
                                        is_client_default = reader.GetBoolean(reader.GetOrdinal("is_client_default")),
                                        last_refreshed_at = reader.GetDateTime(reader.GetOrdinal("last_refreshed_at"))
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Add(LogLevel.Error, "Database operation failed in GetClientColumnDetailsByClientIdAndOmniGuid.", ex);
                        throw;
                    }
            }
            return items;
        }


        /// <summary>
        /// Gets the Client Column info using clientId and omniGuid asynchronous.
        /// </summary>
        /// <param name="clientId">The clientId.</param>
        /// <param name="omniGuid">The omniGuid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<ClientColumnAliases>> GetClientColumnInfoByClientIdAndOmniGuid(
            string? clientId,
            Guid? omniGuid,
            CancellationToken cancellationToken)
        {
            const string allData = "allData";
            var query = (clientId != allData)
      ? @"SELECT omniguid, client_name, client_id, pmds_column_name, columnalias, tier
        FROM planned_media.mv_templates_client_column_aliases
        WHERE UPPER(omniguid) = @OmniGuid AND UPPER(client_name) = @ClientName"
      : @"SELECT omniguid, client_name, client_id, pmds_column_name, columnalias, tier
        FROM planned_media.mv_templates_client_column_aliases";
            var items = new List<ClientColumnAliases>();

            await using (var connection = new NpgsqlConnection(connectionStrings.CurrentValue.AuroraDB))
            {
                await connection.OpenAsync(cancellationToken);
                try
                {
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        if (clientId != allData)
                        {
                            command.Parameters.AddWithValue("@OmniGuid", omniGuid?.ToString().ToUpperInvariant() ?? "");
                            command.Parameters.AddWithValue("@ClientName", clientId?.ToUpperInvariant() ?? "");
                        }
                        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            items.Add(new ClientColumnAliases
                            {
                                omniguid = reader.IsDBNull(reader.GetOrdinal("omniguid")) ? null : reader.GetString(reader.GetOrdinal("omniguid")),
                                client_name = reader.IsDBNull(reader.GetOrdinal("client_name")) ? null : reader.GetString(reader.GetOrdinal("client_name")),
                                client_id = reader.IsDBNull(reader.GetOrdinal("client_id")) ? null : reader.GetString(reader.GetOrdinal("client_id")),
                                pmds_column_name = reader.IsDBNull(reader.GetOrdinal("pmds_column_name")) ? null : reader.GetString(reader.GetOrdinal("pmds_column_name")),
                                columnalias = reader.IsDBNull(reader.GetOrdinal("columnalias")) ? null : reader.GetString(reader.GetOrdinal("columnalias")),
                                tier = reader.IsDBNull(reader.GetOrdinal("tier")) ? null : reader.GetString(reader.GetOrdinal("tier")),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Add(LogLevel.Error, "Database operation failed in GetClientColumnInfoByClientIdAndOmniGuid.", ex);
                    throw;
                }
            }

            return items;
        }

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<AthenaQueryResult> QueryAsync(string query, CancellationToken cancellationToken, bool isSplitQuery = false)
        {
            var (mediaplansquery, mediabriefquery) = isSplitQuery ? GetSplitQuery(query) : ("", "");
            var result = new AthenaQueryResult
            {
                ColumnInfo = new List<ColumnInfo>(),
                Rows = new List<Row>()
            };

            try
            {
                await using var con = new NpgsqlConnection(connectionStrings.CurrentValue.AuroraDB);
                con.Notice += Con_Notice;
                await con.OpenAsync(cancellationToken);

                await using var cmd = new NpgsqlBatch(con);

                if (!isSplitQuery)
                {
                    cmd.BatchCommands.Add(new NpgsqlBatchCommand(query));
                }
                else
                {
                    cmd.BatchCommands.Add(new NpgsqlBatchCommand(mediaplansquery));
                    cmd.BatchCommands.Add(new NpgsqlBatchCommand(mediabriefquery));
                }

                await cmd.PrepareAsync(cancellationToken);

                await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

                // First result set
                var schema = await reader.GetColumnSchemaAsync(cancellationToken);
                var orderedSchema = schema.OrderBy(x => x.ColumnOrdinal).ToList();

                result.ColumnInfo.AddRange(orderedSchema.Select(x => new ColumnInfo
                {
                    Name = x.ColumnName.ToLower(),
                    Nullable = ColumnNullable.UNKNOWN,
                    Type = x.ColumnName.ToLower() == AthenaConsts.Auxiliary || x.ColumnName.ToLower() == AthenaConsts.Source_sys_mandatory
                        ? GetDataType(x.DataTypeName.ToLower())
                        : GetDataType(x.DataType.Name.ToLower()),
                }));

                int totalColumns = result.ColumnInfo.Count;

                while (await reader.ReadAsync(cancellationToken))
                {
                    var row = new Row { Data = new List<Datum>(totalColumns) };
                    foreach (var column in orderedSchema)
                    {
                        var datum = new Datum { VarCharValue = null };
                        if (!reader.IsDBNull(column.ColumnOrdinal!.Value))
                        {
                            datum = GetDatum(reader.GetValue(column.ColumnOrdinal!.Value), column.DataTypeName == "BIT");
                        }
                        row.Data.Add(datum);
                    }
                    result.Rows.Add(row);
                }

                // If split query, process the second result set
                if (isSplitQuery && await reader.NextResultAsync(cancellationToken))
                {
                    var schemaSecond = await reader.GetColumnSchemaAsync(cancellationToken);
                    var orderedSchemaSecond = schemaSecond.OrderBy(x => x.ColumnOrdinal).ToList();

                    result.ColumnInfo.AddRange(orderedSchemaSecond.Select(x => new ColumnInfo
                    {
                        Name = x.ColumnName.ToLower(),
                        Nullable = ColumnNullable.UNKNOWN,
                        Type = GetDataType(x.DataType.Name.ToLower()),
                    }));

                    int secondColumns = orderedSchemaSecond.Count;

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var row = new Row { Data = new List<Datum>(totalColumns + secondColumns) };
                        // Fill with nulls for the first result set columns
                        for (int i = 0; i < totalColumns; i++)
                            row.Data.Add(new Datum { VarCharValue = null });

                        foreach (var column in orderedSchemaSecond)
                        {
                            var datum = new Datum { VarCharValue = null };
                            if (!reader.IsDBNull(column.ColumnOrdinal!.Value))
                            {
                                datum = GetDatum(reader.GetValue(column.ColumnOrdinal!.Value), column.DataTypeName == "BIT");
                            }
                            row.Data.Add(datum);
                        }
                        result.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, "Error executing Aurora query.", ex);
                throw;
            }

            return result;
        }

        private void Con_Notice(object sender, NpgsqlNoticeEventArgs e)
        {
            log.Add(LogLevel.Information, $"Notice: {e.Notice?.Detail}");
        }

        private static string GetDataType(string dataType)
        {
            return dataType switch
            {
                "int16" or "int32" or "int64" => "bigint",
                "single" or "double" or "decimal" => "decimal",
                "string" or "ansistring" or "ansistringfixedlength" or "stringfixedlength" => "string",
                "date" or "datetime2" or "datetimeoffset" or "datetime" => "date",
                "jsonb" or "json" => "jsonb",
                _ => dataType,
            };
        }

        private static Datum GetDatum(object? obj, bool isBit)
        {
            return obj switch
            {
                double dec => new Datum { VarCharValue = dec.ToString(CultureInfo.InvariantCulture) },
                DateTime date => new Datum { VarCharValue = date.ToString("s") + "Z" },
                string _ => new Datum { VarCharValue = obj?.ToString() },
                long l => isBit ? new Datum { VarCharValue = (l == 1).ToString() } : new Datum { VarCharValue = l.ToString() },
                int i => new Datum { VarCharValue = i.ToString() },
                short s => new Datum { VarCharValue = s.ToString() },
                decimal dec => new Datum { VarCharValue = dec.ToString(CultureInfo.InvariantCulture) },
                DateTimeOffset date => new Datum { VarCharValue = date.ToString("s") + "Z" },
                bool b => new Datum { VarCharValue = b.ToString() },
                _ => throw new Exception($"Unexpected type: {obj?.GetType()}"),
            };
        }

        private static (string, string) GetSplitQuery(string query)
        {
            // Defensive: check for expected structure
            int fromIndex = query.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
            int whereIndex = query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
            if (fromIndex < 0 || whereIndex < 0 || whereIndex < fromIndex)
                throw new ArgumentException("Query must contain FROM and WHERE in the correct order.");

            // Extract SELECT columns and WHERE conditions
            string selectPart = query[..fromIndex].Replace("SELECT", "", StringComparison.OrdinalIgnoreCase)
                                                  .Replace("DISTINCT", "", StringComparison.OrdinalIgnoreCase)
                                                  .Trim();
            string fromPart = query[fromIndex..whereIndex].Trim();
            string wherePart = query[(whereIndex + 5)..].Trim(); // 5 = "WHERE".Length

            // Parse columns
            var selectedColumns = new List<string>();
            foreach (var item in selectPart.Split(new[] { "LEAST", "GREATEST" }, StringSplitOptions.TrimEntries))
            {
                if (item.StartsWith('('))
                {
                    if (item.Contains(AthenaConsts.MediaPlansFlightEnd))
                        selectedColumns.Add($"LEAST{item.TrimEnd(',')}");
                    if (item.Contains(AthenaConsts.MediaPlansFlightStart))
                        selectedColumns.Add($"GREATEST{item.TrimEnd(',')}");
                }
                else
                {
                    foreach (var col in item.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        selectedColumns.Add(col.Trim());
                }
            }

            // Parse WHERE conditions
            var whereConditions = wherePart.Split(new[] { " AND ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(c => c.Trim())
                                           .Where(c => c.Length > 0)
                                           .ToList();

            // Helper for column/where selection
            static bool IsMediaPlanCol(string c) =>
                c.StartsWith($"GREATEST({AthenaConsts.MediaPlansTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"LEAST({AthenaConsts.MediaPlansTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.MediaPlansTable, StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase);

            static bool IsMediaBriefCol(string c) =>
                c.StartsWith($"GREATEST({AthenaConsts.MediaBriefsTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"LEAST({AthenaConsts.MediaBriefsTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.MediaBriefsTable, StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase);

            static bool IsMediaPlanWhere(string c) =>
                c.StartsWith($"lower({AthenaConsts.CampaignTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"lower({AthenaConsts.MediaPlansTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"UPPER({AthenaConsts.MediaPlansTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"({AthenaConsts.MediaPlansTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.MediaPlansTable, StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase);

            static bool IsMediaBriefWhere(string c) =>
                c.StartsWith($"lower({AthenaConsts.CampaignTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"lower({AthenaConsts.MediaBriefsTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"UPPER({AthenaConsts.MediaBriefsTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith($"({AthenaConsts.MediaBriefsTable}", StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.MediaBriefsTable, StringComparison.OrdinalIgnoreCase) ||
                c.StartsWith(AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase);

            var mediaPlanColumns = selectedColumns.Where(IsMediaPlanCol).Distinct().ToList();
            var mediaBriefColumns = selectedColumns.Where(IsMediaBriefCol).Distinct().ToList();
            var mediaPlanWhere = whereConditions.Where(IsMediaPlanWhere).Distinct().ToList();
            var mediaBriefWhere = whereConditions.Where(IsMediaBriefWhere).Distinct().ToList();

            // Handle global filters
            var globalFilters = whereConditions.Where(c => (c.StartsWith("(((", StringComparison.Ordinal) || c.EndsWith(")))", StringComparison.Ordinal))).ToList();
            foreach (var filter in globalFilters)
            {
                var campaignGlobalFilters = new List<string>();
                var mediaPlansGlobalFilters = new List<string>();
                var mediaBriefGlobalFilters = new List<string>();

                foreach (var item in filter.Replace("(", "").Replace(")", "").Split(" AND ", StringSplitOptions.RemoveEmptyEntries))
                {
                    foreach (var orCondition in item.Split(" OR ", StringSplitOptions.RemoveEmptyEntries))
                    {
                        var cond = orCondition.Trim();
                        if (cond.StartsWith(AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase))
                            campaignGlobalFilters.Add(cond);
                        else if (cond.StartsWith(AthenaConsts.MediaPlansTable, StringComparison.OrdinalIgnoreCase))
                            mediaPlansGlobalFilters.Add(cond);
                        else if (cond.StartsWith(AthenaConsts.MediaBriefsTable, StringComparison.OrdinalIgnoreCase))
                            mediaBriefGlobalFilters.Add(cond);
                    }
                }
                if (campaignGlobalFilters.Count > 0)
                {
                    mediaPlanWhere.Add($"({string.Join(" OR ", campaignGlobalFilters)})");
                    mediaBriefWhere.Add($"({string.Join(" OR ", campaignGlobalFilters)})");
                }
                if (mediaPlansGlobalFilters.Count > 0)
                    mediaPlanWhere.Add($"({string.Join(" OR ", mediaPlansGlobalFilters)})");
                if (mediaBriefGlobalFilters.Count > 0)
                    mediaBriefWhere.Add($"({string.Join(" OR ", mediaBriefGlobalFilters)})");
            }

            // Compose queries
            var mediaPlansQuery = $"SELECT {string.Join(", ", mediaPlanColumns)} {TableJoinMediaPlans} WHERE {string.Join(" AND ", mediaPlanWhere)}";
            var mediaBriefQuery = $"SELECT {string.Join(", ", mediaBriefColumns)} {TableJoinMediaBriefs} WHERE {string.Join(" AND ", mediaBriefWhere)}";
            return (mediaPlansQuery, mediaBriefQuery);
        }

    }
}