using Amazon.Athena;
using Amazon.Athena.Model;
using Lib.Athena.Models;
using Lib.Aurora.Business.Interfaces;
using Lib.Aurora.Consts;
using Lib.Aurora.Models;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.MediaopsToFlowChart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Globalization;
using System.Reflection;
using Column = Lib.MediaopsToFlowChart.Models.Column;

namespace Lib.SampleData.Business
{
    /// <summary>
    /// AthenaSampleDataQueryLogic
    /// </summary>
    public class AthenaSampleDataQueryLogic : IAuroraQueryLogic
    {
        private readonly ILog<AthenaSampleDataQueryLogic> log;
        private readonly SampleDataDbContext sampleData;

        /// <summary>
        /// Initializes a new instance of the <see cref="AthenaSampleDataQueryLogic" />
        /// class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="sampleData">The sample data.</param>
        public AthenaSampleDataQueryLogic(ILog<AthenaSampleDataQueryLogic> log, SampleDataDbContext sampleData)
        {
            this.log = log;
            this.sampleData = sampleData;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static T GetData<T>(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var file = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(fileName))
                ?? throw new Exception($"No data file found: {fileName}");

            using var stream = assembly.GetManifestResourceStream(file)
                ?? throw new Exception("Could not get manifest resource stream.");

            using var reader = new StreamReader(stream);

            var text = reader.ReadToEnd();

            return Json.Deserialize<T>(text)
                ?? throw new Exception("Invalid data.");
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
        /// <param name="omniGuid">The table.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<Row>> GetDisplayNameFromClientColumnAliasesTable(Guid omniGuid, CancellationToken cancellationToken)
        {
            var query = $"SELECT Distinct source_column_name as columnname, columnalias FROM planned_media.templates_client_column_aliases WHERE UPPER(omniguid) = '{omniGuid.ToString().ToUpper()}'";
            var result = await QueryAsync(query, cancellationToken);
            return result.Rows;
        }

        /// <summary>
        /// Gets the client column details using clientId and omniGuid asynchronous.
        /// </summary>
        /// <param name="clientId">The table.</param>
        /// <param name="omniGuid">The omniGuid</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<TemplatesClientColumnAliases>> GetClientColumnDetailsByClientIdAndOmniGuid(string clientId, Guid omniGuid, CancellationToken cancellationToken)
        {
            return [];//not used
        }

        /// <summary>
        /// Gets the Client Column info using clientId and omniGuid asynchronous.
        /// </summary>
        /// <param name="clientId">The clientId.</param>
        /// <param name="omniGuid">The omniGuid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<ClientColumnAliases>> GetClientColumnInfoByClientIdAndOmniGuid(string? clientId, Guid? omniGuid, CancellationToken cancellationToken)
        {
            return [];//not used
        }

        /// <summary>
        /// Queries the asynchronous.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="isSplitQuery">The isSplitQuery.</param>
        public async Task<AthenaQueryResult> QueryAsync(string query, CancellationToken cancellationToken, bool isSplitQuery = false)
        {
            log.Add(LogLevel.Information, query);

            var result = new AthenaQueryResult
            {
                Rows = [],
            };

            using (var command = sampleData.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                await sampleData.Database.OpenConnectionAsync(cancellationToken);
                using var sqlResult = await command.ExecuteReaderAsync(cancellationToken);

                var schema = await sqlResult.GetColumnSchemaAsync(cancellationToken);

                result.ColumnInfo = schema.OrderBy(x => x.ColumnOrdinal).Select(x => new ColumnInfo
                {
                    Name = x.ColumnName,
                    Nullable = ColumnNullable.UNKNOWN,
                    Type = GetAthenaType(x.DataTypeName!.ToLower()),
                }).ToList();

                while (await sqlResult.ReadAsync(cancellationToken))
                {
                    var row = new Row { Data = new List<Datum>() };

                    result.Rows.Add(row);

                    foreach (var column in schema.OrderBy(x => x.ColumnOrdinal))
                    {
                        var datum = new Datum { VarCharValue = null };

                        if (!sqlResult.IsDBNull(column.ColumnOrdinal!.Value))
                        {
                            datum = GetDatum(sqlResult.GetValue(column.ColumnOrdinal!.Value), column.DataTypeName == "BIT");
                        }

                        row.Data.Add(datum);
                    }
                }
            }

            return result;
        }

        private string GetAthenaType(string dataType)
        {
            return dataType switch
            {
                "bit" => "boolean",
                _ => dataType,
            };
        }

        private Datum GetDatum(object? obj, bool isBit)
        {
            switch (obj)
            {
                case double dec:
                    return new Datum { VarCharValue = dec.ToString(CultureInfo.InvariantCulture) };

                case DateTime date:
                    return new Datum { VarCharValue = date.ToString("s") + "Z" };

                case string _:
                    return new Datum { VarCharValue = obj?.ToString() };

                case long l:
                    if (isBit)
                    {
                        return new Datum { VarCharValue = (l == 1).ToString() };
                    }
                    return new Datum { VarCharValue = l.ToString() };

                default:
                    throw new Exception($"Unexpected type: {obj?.GetType()}");
            }
        }
    }
}