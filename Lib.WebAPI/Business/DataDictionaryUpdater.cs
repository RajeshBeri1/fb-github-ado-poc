using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Lib.Athena.Business;
using Lib.Athena.Business.Interfaces;
using Lib.Athena.Consts;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Linq;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DataDictionaryUpdater
    /// </summary>
    public class DataDictionaryUpdater
    {
        /// <summary>
        /// The tables
        /// </summary>
        public static string[] Tables = { AthenaConsts.MediaPlansTable, AthenaConsts.CampaignTable, AthenaConsts.MediaBriefsTable, AthenaConsts.Channel, AthenaConsts.Supplier, AthenaConsts.Placement, AthenaConsts.Budget };

        public static string[] PmdsNewTables = { AthenaConsts.Channel, AthenaConsts.Supplier, AthenaConsts.Placement, AthenaConsts.Budget };

        private readonly AthenaDataConverter dataConverter;
        private readonly ILog<DataDictionaryUpdater> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly IAuroraQueryLogic queryLogic;
        private readonly IConfiguration configuration;
        private readonly PortalDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDictionaryUpdater" /> class.
        /// </summary>
        /// <param name="queryLogic">The athena query logic.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="portal">The portal.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="log">The log.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="dbContext">The dbContext.</param>
        public DataDictionaryUpdater(
            IAuroraQueryLogic queryLogic,
            IMapper mapper,
            IPortalUnitOfWork portal,
            AthenaDataConverter dataConverter,
            ILog<DataDictionaryUpdater> log,
            IConfiguration configuration,
            PortalDbContext dbContext)
        {
            this.queryLogic = queryLogic;
            this.mapper = mapper;
            this.portal = portal;
            this.dataConverter = dataConverter;
            this.log = log;
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Update started.");

            foreach (var table in Tables.Where(x=> !PmdsNewTables.Any(y=> y==x)))
            {
                await UpdateTableAsync(table, cancellationToken);
            }

            await UpdateTableAsync(AthenaConsts.ClientRollPeriodTable, cancellationToken);
            await UpdateTableAsync(AthenaConsts.RollPeriodTable, cancellationToken);

            await UpdateClientAliasesAsync(cancellationToken);

            await UpdateClientCalendarAsync(cancellationToken);

            await UpdateBroadcastCalendarAsync(cancellationToken);

            // Disabled client and omni client refresh due to performance and data issue for live connection
            if (configuration.GetValue<bool>("UseSampleData"))
            {
                await UpdateClientsAsync(cancellationToken);
            }

            log.Add(LogLevel.Information, $"Update finished.");
        }

        private T GetData<T>(Dictionary<string, object?> data, string field)
        {
            if (!data.ContainsKey(field))
            {
                throw new Exception($"Data has no key {field}.");
            }

            if ((Nullable.GetUnderlyingType(typeof(T)) != null || typeof(T) == typeof(string)) && data[field] == null)
            {
                return default!;
            }

            if (data[field] is T value)
            {
                return value;
            }

            throw new Exception($"Type mismatch for field {field}, {typeof(T)} expected, actual type: {data[field]?.GetType()}");
        }

        private async Task UpdateBroadcastCalendarAsync(CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating broadcast calendar.");

            var result = await queryLogic.QueryAsync($"SELECT * FROM {AthenaConsts.RollPeriodTable}", cancellationToken);

            var data = dataConverter.Convert(result);

            portal.BroadcastCalendars.Remove();
            await portal.SaveChangesAsync(cancellationToken);

            var years = data.GroupBy(x => x[AthenaConsts.RollPeriodYearNumber]);

            foreach (var year in years.Where(x => x.Key != null))
            {
                var y = (long)year.Key!;

                await portal.BroadcastCalendars.AddAsync(
                    new BroadcastCalendar
                    {
                        Year = y,
                        BroadcastCalendarItems = Json.Serialize(
                            year.Select(x => x.Keys.Select(k => new CalendarItem
                            {
                                ColumnName = k,
                                JsonValue = Json.Serialize(x[k]),
                            })).ToList()),
                    }, cancellationToken);
            }

            await portal.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateClientAliasesAsync(CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating client column aliases.");

            var result = await queryLogic.QueryAsync($"SELECT * FROM {AthenaConsts.ClientColumnAliasesTable}", cancellationToken);

            var data = dataConverter.Convert(result);

            portal.ClientColumnAliases.Remove();
            await portal.SaveChangesAsync(cancellationToken);

            var clients = data.GroupBy(x => x[AthenaConsts.ClientId]);

            foreach (var client in clients)
            {
                var clientId = client.Key as string;

                await portal.ClientColumnAliases.AddAsync(
                    new ClientColumnAlias
                    {
                        ClientId = clientId!,
                        ColumnAliases = Json.Serialize(
                            client.Where(x =>
                            x[AthenaConsts.AliasesColumnAlias] != null &&
                            x[AthenaConsts.AliasesColumnName] != null &&
                            x[AthenaConsts.AliasesTableName] != null
                            ).Select(x => new ColumnAlias
                            {
                                Alias = GetData<string>(x, AthenaConsts.AliasesColumnAlias),
                                ColumnName = GetData<string>(x, AthenaConsts.AliasesColumnName),
                                TableName = GetData<string>(x, AthenaConsts.AliasesTableName),
                            }).ToList()),
                    }, cancellationToken);
            }

            await portal.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateClientCalendarAsync(CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating client calendar.");

            var result = await queryLogic.QueryAsync($"SELECT * FROM {AthenaConsts.ClientRollPeriodTable}", cancellationToken);

            portal.ClientCalendars.Remove();
            await portal.SaveChangesAsync(cancellationToken);

            var data = dataConverter.Convert(result);

            var clients = data.GroupBy(x => x[AthenaConsts.ClientId]);

            foreach (var client in clients)
            {
                var clientId = client.Key as string;

                var years = client.ToList().GroupBy(x => x[AthenaConsts.ClientRollPeriodClientYear]);

                foreach (var year in years)
                {
                    var clientYear = year.Key as string;

                    await portal.ClientCalendars.AddAsync(
                        new ClientCalendar
                        {
                            ClientId = clientId!,
                            ClientYear = clientYear!,
                            ClientCalendarItems = Json.Serialize(
                                year.Select(x => x.Keys.Select(k => new CalendarItem
                                {
                                    ColumnName = k,
                                    JsonValue = Json.Serialize(x[k]),
                                })).ToList()),
                        }, cancellationToken);
                }
            }

            await portal.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateClientsAsync(CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating clients.");

            var result = await queryLogic.QueryAsync($"SELECT * FROM {AthenaConsts.ClientTable}", cancellationToken);

            portal.OmniClients.Remove();
            portal.Clients.Remove();
            await portal.SaveChangesAsync(cancellationToken);

            var data = dataConverter.Convert(result);
            // Temporary untill the Omni Guid issue in Athen is fixed
            var allowedClients = new List<string>();
            configuration.GetSection("Clients").Bind(allowedClients);
            if (allowedClients != null && allowedClients.Any())
            {
                data = data.Where(c => allowedClients.Any(ac => ac == c[AthenaConsts.ClientName] as string)).ToList();
            }
            foreach (var row in data)
            {
                var id = row[AthenaConsts.ClientId] as string;
                var name = row[AthenaConsts.ClientName] as string;

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var query = $@"SELECT DISTINCT {AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid} AS {AthenaConsts.OmniGuid},
                                {AthenaConsts.CampaignTable}.{AthenaConsts.Country} AS {AthenaConsts.Country}
                                {DataControllerLogic.TableJoinWithPlanId} WHERE
                                {AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid} IS NOT NULL AND
                                {AthenaConsts.CampaignTable}.{AthenaConsts.ClientName} = {DataControllerLogic.SqlQuote(name)}";

                result = await queryLogic.QueryAsync(query, cancellationToken);

                var clientData = dataConverter.Convert(result);

                var omniClients = clientData.Select(x => new
                {
                    guid = x[AthenaConsts.OmniGuid] as string,
                    country = x[AthenaConsts.Country] as string,
                }).Where(x => Guid.TryParse(x.guid, out var _) && !string.IsNullOrWhiteSpace(x.country))
                .Select(x => new OmniClient
                {
                    Id = Guid.Parse(x.guid!),
                    Country = x.country!,
                }).DistinctBy(c => c.Id).ToList();

                query = $@"SELECT DISTINCT {AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid} AS {AthenaConsts.OmniGuid},
                                {AthenaConsts.CampaignTable}.{AthenaConsts.Country} AS {AthenaConsts.Country}
                                {DataControllerLogic.TableJoinWithPlanId} WHERE
                                {AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid} IS NOT NULL AND
                                {AthenaConsts.CampaignTable}.{AthenaConsts.ClientName} = {DataControllerLogic.SqlQuote(name)}";

                if (omniClients.Any())
                {
                    query += $@" AND UPPER({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid})
                                NOT IN ({string.Join(",", omniClients.Select(x => $"{DataControllerLogic.SqlQuote(x.Id.ToString().ToUpper())}"))})";
                }

                result = await queryLogic.QueryAsync(query, cancellationToken);

                clientData = dataConverter.Convert(result);

                omniClients.AddRange(
                    clientData.Select(x => new
                    {
                        guid = x[AthenaConsts.OmniGuid] as string,
                        country = x[AthenaConsts.Country] as string,
                    }).Where(x => Guid.TryParse(x.guid, out var _) && !string.IsNullOrWhiteSpace(x.country))
                .Select(x => new OmniClient
                {
                    Id = Guid.Parse(x.guid!),
                    Country = x.country!,
                }).DistinctBy(c => c.Id));

                await portal.Clients.AddAsync(
                    new Client
                    {
                        ClientId = id,
                        Name = name,
                        OmniClients = omniClients,
                    }, cancellationToken);
            }

            await portal.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateClientDetailsByOmniClientIdAsync(Guid omniClientId, string clientId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating clients by OmniClientId");

            var result = await queryLogic.QueryAsync($"SELECT * FROM {AthenaConsts.ClientTable} where client_id = '{clientId}'", cancellationToken);

            dbContext.Remove(dbContext.OmniClients.FirstOrDefault(a => a.Id == omniClientId));
            dbContext.Remove(dbContext.Clients.FirstOrDefault(a => a.ClientId == clientId));

            await dbContext.SaveChangesAsync(cancellationToken);

            var data = dataConverter.Convert(result);
            // Temporary untill the Omni Guid issue in Athen is fixed
            var allowedClients = new List<string>();
            configuration.GetSection("Clients").Bind(allowedClients);
            if (allowedClients != null && allowedClients.Any())
            {
                data = data.Where(c => allowedClients.Exists(ac => ac == c[AthenaConsts.ClientName] as string)).ToList();
            }
            foreach (var row in data)
            {
                var id = row[AthenaConsts.ClientId] as string;
                var name = row[AthenaConsts.ClientName] as string;

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var query = $@"SELECT DISTINCT {AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid} AS {AthenaConsts.OmniGuid},
                                {AthenaConsts.CampaignTable}.{AthenaConsts.Country} AS {AthenaConsts.Country}
                                {DataControllerLogic.TableJoinWithPlanId} WHERE
                                {AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid} IS NOT NULL AND
                                {AthenaConsts.CampaignTable}.{AthenaConsts.ClientName} = {DataControllerLogic.SqlQuote(name)}";

                result = await queryLogic.QueryAsync(query, cancellationToken);

                var clientData = dataConverter.Convert(result);

                var omniClients = clientData.Select(x => new
                {
                    guid = x[AthenaConsts.OmniGuid] as string,
                    country = x[AthenaConsts.Country] as string,
                }).Where(x => Guid.TryParse(x.guid, out var _) && !string.IsNullOrWhiteSpace(x.country))
                .Select(x => new OmniClient
                {
                    Id = Guid.Parse(x.guid!),
                    Country = x.country!,
                }).DistinctBy(c => c.Id).ToList();

                query = $@"SELECT DISTINCT {AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid} AS {AthenaConsts.OmniGuid},
                                {AthenaConsts.CampaignTable}.{AthenaConsts.Country} AS {AthenaConsts.Country}
                                {DataControllerLogic.TableJoinWithPlanId} WHERE
                                {AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid} IS NOT NULL AND
                                {AthenaConsts.CampaignTable}.{AthenaConsts.ClientName} = {DataControllerLogic.SqlQuote(name)}";

                if (omniClients.Any())
                {
                    query += $@" AND UPPER({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid})
                                NOT IN ({string.Join(",", omniClients.Select(x => $"{DataControllerLogic.SqlQuote(x.Id.ToString().ToUpper())}"))})";
                }

                result = await queryLogic.QueryAsync(query, cancellationToken);

                clientData = dataConverter.Convert(result);

                omniClients.AddRange(
                    clientData.Select(x => new
                    {
                        guid = x[AthenaConsts.OmniGuid] as string,
                        country = x[AthenaConsts.Country] as string,
                    }).Where(x => Guid.TryParse(x.guid, out var _) && !string.IsNullOrWhiteSpace(x.country))
                .Select(x => new OmniClient
                {
                    Id = Guid.Parse(x.guid!),
                    Country = x.country!,
                }).DistinctBy(c => c.Id));

                await portal.Clients.AddAsync(
                    new Client
                    {
                        ClientId = id,
                        Name = name,
                        OmniClients = omniClients,
                    }, cancellationToken);
            }

            await portal.SaveChangesAsync(cancellationToken);
        }


        private async Task UpdateTableAsync(string table, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating table {table}.");

            var dbTable = await portal.DataDictionaryTables.GetByNameAsync(table, cancellationToken);

            var columnInfos = await queryLogic.GetTableColumnInfosAsync(table, cancellationToken);

            dbTable.DataDictionaryColumns = Json.Serialize(
                columnInfos.Select(x => mapper.Map<DataDictionaryColumn>(x)).ToList());

            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Refresh the DataDictionary Tables data manually asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> RefreshDataDictionaryTablesAsync(CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Refreshing the Data Dictionary Tables started.");

            Stopwatch stopWatch = new Stopwatch();

            log.Add(LogLevel.Information, $"Manual refreshing of DataDictionary tables data started at: {DateTime.UtcNow}");
            stopWatch.Start();
            foreach (var table in Tables)
            {
                await UpdateTableAsync(table, cancellationToken);
            }

            await UpdateTableAsync(AthenaConsts.ClientRollPeriodTable, cancellationToken);
            await UpdateTableAsync(AthenaConsts.RollPeriodTable, cancellationToken);

            await UpdateClientAliasesAsync(cancellationToken);

            await UpdateClientCalendarAsync(cancellationToken);

            await UpdateBroadcastCalendarAsync(cancellationToken);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string waitTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            log.Add(LogLevel.Information, $"Manual refreshing of DataDictionary tables data ended at: {DateTime.UtcNow}");
            log.Add(LogLevel.Information, $"Stopwatch Time for Manual refreshing of DataDictionary tables data is: {waitTime}");
            return $"DataDictionary Tables Data Refreshed Successfully and time taken is: {waitTime}";
        }
    }
}