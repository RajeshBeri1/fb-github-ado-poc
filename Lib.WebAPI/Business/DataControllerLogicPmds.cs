using AutoMapper;
using JasperFx.Core;
using Lib.Athena.Business;
using Lib.Athena.Consts;
using Lib.Athena.Enumerations;
using Lib.Athena.Models;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.MediaopsToFlowChart.Business;
using Lib.MediaopsToFlowChart.DTO;
using Lib.MediaopsToFlowChart.Models;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Helpers;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Data;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Throw;
using Column = Lib.MediaopsToFlowChart.Models.Column;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DataControllerLogicPmds
    /// </summary>
    public partial class DataControllerLogicPmds : CommonDataControllerLogic
    {
        [GeneratedRegex(@"lvl\d*_channel*", RegexOptions.Compiled)]
        private static partial Regex LevelChannelRegex();

        [GeneratedRegex(@"sblvl_\d*_\d*_\d*_channel*", RegexOptions.Compiled)]
        private static partial Regex SubLevelChannelRegex();

        [GeneratedRegex(@"\(\s*budget_auxiliary\s*->>\s*(?:'briefedctc_local'|'briefedctc'|'budget_local'|'budget')\s*\)(::float\s+AS\s*.*)", RegexOptions.Compiled)]
        private static partial Regex BudgetBriefedCtcMetricZeroRegex();

        [GeneratedRegex(@"\(.*?_auxiliary->>'(.*?)'\)\:\:float", RegexOptions.Compiled)]
        private static partial Regex MetricsZeroRegex();

        [GeneratedRegex(@"(.*?)\s+as\s+(lvlmtc_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex LevelMetricPattern();

        [GeneratedRegex(@"(.*?)\s+as\s+(sblvlmtc_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex SubLevelMetricPattern();

        [GeneratedRegex(@"(.*?)\s+as\s+(lvlsbt_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex SubTotalMetricPattern();

        [GeneratedRegex(@"(.*?)\s+as\s+(lvlsbtsmrymtc_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex SubTotalSummaryMetricPattern();

        [GeneratedRegex(@"(.*?)\s+as\s+(gt_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex GrandTotalMetricPattern();

        [GeneratedRegex(@"(.*?)\s+as\s+(total\d+[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex RightHandTotalMetricPattern();

        [GeneratedRegex(@"lvlmtc_\d+(_\d+)*_(.*)$", RegexOptions.Compiled)]
        private static partial Regex LevelMetricPatternForSelect();

        [GeneratedRegex(@"lvlsbt_\d+(_\d+)*_(.*)$", RegexOptions.Compiled)]
        private static partial Regex LevelSubTotalMetricPatternForSelect();

        [GeneratedRegex(@"gt(_\d+)*_(.*)$", RegexOptions.Compiled)]
        private static partial Regex GrandTotalMetricPatternForSelect();

        [GeneratedRegex(@"lvlsbtsmrymtc_\d+(_\d+)*_(.*)$", RegexOptions.Compiled)]
        private static partial Regex LevelSubTotalSummaryMetricPatternForSelect();

        [GeneratedRegex(@"total(\d+)*_(.*)$", RegexOptions.Compiled)]

        private static partial Regex TotalsMetricPatternForSelect();

        [GeneratedRegex(@"(.*?)\s+as\s+(mtc_[^\s]+)", RegexOptions.Compiled)]
        private static partial Regex GenericMetricPattern();

        [GeneratedRegex(@"\(budget_auxiliary->>'briefedctc(_local)?'\)(::float\s+AS\s+total_briefedctc)", RegexOptions.Compiled)]

        private static partial Regex TotalBriefedCtcOrLocalZeroPatternForSelect();

        [GeneratedRegex(@"(\w+_auxiliary\s*->>\s*'(impressions|impressionsactual|budgetedimpressions)')\s+as\s+([^\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
        private static partial Regex SupplierImpressionsZeroRegex();

        [GeneratedRegex(@"lvlifo(?:_\d+)+_(\w+)", RegexOptions.Compiled)]
        private static partial Regex InflightRegex();


        /// <summary>
        /// MediaToolsSource
        /// </summary>
        private const string MediaToolsSource = "MediaTools";

        /// <summary>
        /// SourceSysMandatoryField
        /// </summary>
        private const string SourceSysMandatoryField = "source_sys_mandatory";
        /// <summary>
        /// ColumnDetails
        /// </summary>
        public record ColumnDetails(string ColumnName, bool IsCurrency);

        /// <summary>
        /// Fields to map for metric and non currency columns
        /// </summary>
        public static readonly string MetricColumnsWithNonCurrency = "[{\"ColumnName\":\"actualctc\",\"IsCurrency\":true},{\"ColumnName\":\"actualctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"actualnetmedia\",\"IsCurrency\":true},{\"ColumnName\":\"actualnetmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"actuals\",\"IsCurrency\":true},{\"ColumnName\":\"actuals_local\",\"IsCurrency\":true},{\"ColumnName\":\"addedvalue\",\"IsCurrency\":true},{\"ColumnName\":\"addedvalue_local\",\"IsCurrency\":true},{\"ColumnName\":\"admissions\",\"IsCurrency\":true},{\"ColumnName\":\"adserving\",\"IsCurrency\":true},{\"ColumnName\":\"adserving_local\",\"IsCurrency\":true},{\"ColumnName\":\"adverificationcost\",\"IsCurrency\":true},{\"ColumnName\":\"adverificationcost_local\",\"IsCurrency\":true},{\"ColumnName\":\"asbof\",\"IsCurrency\":true},{\"ColumnName\":\"asbof_local\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"commission\",\"IsCurrency\":true},{\"ColumnName\":\"commission_local\",\"IsCurrency\":true},{\"ColumnName\":\"dst\",\"IsCurrency\":true},{\"ColumnName\":\"dst_local\",\"IsCurrency\":true},{\"ColumnName\":\"eqgrps\",\"IsCurrency\":false},{\"ColumnName\":\"footfalls\",\"IsCurrency\":false},{\"ColumnName\":\"grossmedia\",\"IsCurrency\":true},{\"ColumnName\":\"grossmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"grps\",\"IsCurrency\":false},{\"ColumnName\":\"impressions\",\"IsCurrency\":false},{\"ColumnName\":\"installation\",\"IsCurrency\":true},{\"ColumnName\":\"installation_local\",\"IsCurrency\":true},{\"ColumnName\":\"localtax1\",\"IsCurrency\":true},{\"ColumnName\":\"localtax1_local\",\"IsCurrency\":true},{\"ColumnName\":\"localtax2\",\"IsCurrency\":true},{\"ColumnName\":\"localtax2_local\",\"IsCurrency\":true},{\"ColumnName\":\"medialevycost\",\"IsCurrency\":true},{\"ColumnName\":\"medialevycost_local\",\"IsCurrency\":true},{\"ColumnName\":\"mediaops\",\"IsCurrency\":true},{\"ColumnName\":\"mediaops_local\",\"IsCurrency\":true},{\"ColumnName\":\"netmedia\",\"IsCurrency\":true},{\"ColumnName\":\"netmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"othercost\",\"IsCurrency\":true},{\"ColumnName\":\"othercost_local\",\"IsCurrency\":true},{\"ColumnName\":\"plannedctc\",\"IsCurrency\":true},{\"ColumnName\":\"plannedctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"plannednetmedia\",\"IsCurrency\":true},{\"ColumnName\":\"plannednetmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"population\",\"IsCurrency\":false},{\"ColumnName\":\"pretaxtotal\",\"IsCurrency\":true},{\"ColumnName\":\"pretaxtotal_local\",\"IsCurrency\":true},{\"ColumnName\":\"production\",\"IsCurrency\":true},{\"ColumnName\":\"production_local\",\"IsCurrency\":true},{\"ColumnName\":\"screens\",\"IsCurrency\":false},{\"ColumnName\":\"spots\",\"IsCurrency\":false},{\"ColumnName\":\"tacticalctc\",\"IsCurrency\":true},{\"ColumnName\":\"tacticalctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"totalclient\",\"IsCurrency\":true},{\"ColumnName\":\"totalclient_local\",\"IsCurrency\":true},{\"ColumnName\":\"totalfees\",\"IsCurrency\":true},{\"ColumnName\":\"totalfees_local\",\"IsCurrency\":true},{\"ColumnName\":\"totalnet\",\"IsCurrency\":true},{\"ColumnName\":\"totalnet_local\",\"IsCurrency\":true},{\"ColumnName\":\"totaltax\",\"IsCurrency\":true},{\"ColumnName\":\"totaltax_local\",\"IsCurrency\":true},{\"ColumnName\":\"unitrate\",\"IsCurrency\":true},{\"ColumnName\":\"unitrate_local\",\"IsCurrency\":true},{\"ColumnName\":\"units\",\"IsCurrency\":false},{\"ColumnName\":\"vat\",\"IsCurrency\":true},{\"ColumnName\":\"vat_local\",\"IsCurrency\":true}]";

        /// <summary>
        /// MetricCurrencyColumns
        /// </summary>
#pragma warning disable CS8604 // Possible null reference argument.
        public static readonly string[] MetricCurrencyColumns = Json.Deserialize<Collection<ColumnDetails>>(MetricColumnsWithNonCurrency).Where(x => x.IsCurrency).Select(x => x.ColumnName).ToArray();
#pragma warning restore CS8604 // Possible null reference argument.


        /// <summary>
        /// DateTime with timestamp fields
        /// </summary>
        public static readonly string[] TimeStampWithTimeZoneColumns = new string[]
        {
            "amd_timestamp",
            "campaignstart",
            "campaignend",
            "file_date",
            "mygrid_flightstartdate",
            "mygrid_flightenddate",
            "effective_date",
            "approvaldeadline",
            "flightstart",
            "flightend",
            "week_text",
            "bookingdeadline",
            "materialclose",
            "issuedate",
            "onsaledate",
        };

        private const string EffectiveDateKey = $"{AthenaConsts.Supplier}_{AthenaConsts.MediaPlansEffectiveDate}"; //$"{AthenaConsts.Supplier}_{AthenaConsts.MediaPlansEffectiveDate}";
        private const string FlightEndKey = $"{AthenaConsts.Supplier}_{AthenaConsts.MediaPlansFlightEnd}";
        private const string FlightStartKey = $"{AthenaConsts.Supplier}_{AthenaConsts.MediaPlansFlightStart}";
        private const string GrandTotalsPrefix = "gt";
        private const string HeaderPrefix = "header";
        private const string LevelInflightOverlayPrefix = "lvlifo";
        private const string LevelMetricPrefix = "lvlmtc";
        private const string LevelSubTotalPrefix = "lvlsbt";
        private const string MediaHierarchyLevelPrefix = "lvl";
        private const string MediaHierarchySubLevelPrefix = "sblvl";
        private const string SubLevelInflightOverlayPrefix = "sblvlifo";
        private const string SubLevelMetricPrefix = "sblvlmtc";
        private const string SubLevelSubTotalPrefix = "sblvlsbt";
        private const string SystemCampaignIdKey = $"{AthenaConsts.CampaignTable}_{AthenaConsts.SystemCampaignId}";
        private const string TotalsPrefix = "total";
        private const string CampaignLocalCurrencyKey = $"{AthenaConsts.CampaignTable}_{AthenaConsts.CampaignLocalCurrencyName}";
        private const string LegendPrefix = "legend";
        private const string DateFormat = "MM/dd";
        private const string CustomFlightRange = "CustomFlightRange";
        private const string LevelSubTotalSummaryPrefix = "lvlsbtsmry";
        private const string LevelSubTotalSummaryMetricPrefix = "lvlsbtsmrymtc";
        private const string GenericMetricPrefix = "mtc";
        private const string GenericInflightOverlayPrefix = "inflight";

        private static readonly Guid DefaultMetricTableId = Guid.Parse("0d771867-cc9a-40a5-900e-f57528f91d0e");
        private static readonly string DefaultMetricColumnName = "netmedia";

        private readonly ICacheLogic cache;
        private readonly IPortalDbContext portalDbContext;
        private readonly AthenaDataConverter dataConverter;
        private readonly ILog<DataControllerLogicPmds> log;
        private readonly IMapper mapper;
        private readonly OmniAuthConfig omniAuthConfig;
        private readonly IPortalUnitOfWork portal;
        private readonly IAuroraQueryLogic queryLogic;
        private readonly SecurityLogic securityLogic;
        private readonly IConfiguration configuration;
        private readonly MediaopsToFlowChartImportApi mediaopsToFlowChartImportApi;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly MediaopsControllerLogic mediaopsControllerLogic;
        private readonly IClientControllerLogic clientControllerLogic;
        private readonly IUserProvider userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataControllerLogicPmds" /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="queryLogic">The query logic.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        /// <param name="log">The log.</param>
        /// <param name="omniAuthConfig">The omni authentication configuration.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mediaopsToFlowChartImportApi">The mediaopsToFlowChartImportApi.</param>
        /// <param name="clientControllerLogic">The clientControllerLogic</param>
        /// <param name="mediaopsControllerLogic">The mediaopsControllerLogic</param>
        /// <param name="userProvider">The userProvider</param>
        /// <param name="httpContextAccessor">The httpContextAccessor</param>
        public DataControllerLogicPmds(
                IPortalUnitOfWork portal,
                IAuroraQueryLogic queryLogic,
                ICacheLogic cache,
                IPortalDbContext portalDbContext,
                ILog<DataControllerLogicPmds> log,
                OmniAuthConfig omniAuthConfig,
                SecurityLogic securityLogic,
                IMapper mapper,
                AthenaDataConverter dataConverter,
                IConfiguration configuration,
                MediaopsToFlowChartImportApi mediaopsToFlowChartImportApi,
                IHttpContextAccessor httpContextAccessor,
                MediaopsControllerLogic mediaopsControllerLogic,
                IClientControllerLogic clientControllerLogic,
                IUserProvider userProvider)
            : base(portal, omniAuthConfig)
        {
            this.portal = portal;
            this.queryLogic = queryLogic;
            this.cache = cache;
            this.portalDbContext = portalDbContext;
            this.log = log;
            this.omniAuthConfig = omniAuthConfig;
            this.securityLogic = securityLogic;
            this.mapper = mapper;
            this.dataConverter = dataConverter;
            this.configuration = configuration;
            this.mediaopsToFlowChartImportApi = mediaopsToFlowChartImportApi;
            this.httpContextAccessor = httpContextAccessor;
            this.mediaopsControllerLogic = mediaopsControllerLogic;
            this.clientControllerLogic = clientControllerLogic;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Gets the broadcast calendar asynchronous.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetBroadcastCalendarAsync(long year, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting broadcast calendar for user {userId}.");

            var calendar = (await portal.BroadcastCalendars.GetAsync(cancellationToken, predicate: x => x.Year == year)).FirstOrDefault()
                ?? throw new KeyNotFoundException($"Year {year} not found.");

            return Json.Deserialize<IEnumerable<IEnumerable<CalendarItem>>>(calendar.BroadcastCalendarItems)
                ?? throw new Exception($"Deserialisation failed.");
        }

        /// <summary>
        /// Gets the client calendar asynchronous.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetClientCalendarAsync(string year, Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting client calendar for user {userId}.");

            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);

            var clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);

            var calendar = (await portal.ClientCalendars.GetAsync(cancellationToken, count: 1, predicate: x => x.ClientYear == year && x.ClientId == clientId)).FirstOrDefault()
                ?? throw new KeyNotFoundException($"No client calendar found for client {clientId} and year {year}.");

            return Json.Deserialize<IEnumerable<IEnumerable<CalendarItem>>>(calendar.ClientCalendarItems)
                ?? throw new Exception($"Deserialisation failed.");
        }

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <param name="dataRequest">The data request.</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartData> GetDataAsync(DataRequestDTO dataRequest, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting data for user {userId}.");

            dataRequest.FlowchartDefinition.ThrowIfNull();
            dataRequest.FlowchartDefinition.CalendarDefinition.ThrowIfNull();
            dataRequest.FlowchartDefinition.CalendarDefinition.Definition.ThrowIfNull();
            dataRequest.FlowchartDefinition.CalendarDefinition.Definition.Configuration.ThrowIfNull();
            dataRequest.FlowchartDefinition.MediaHierarchyDefinition.ThrowIfNull();
            dataRequest.FlowchartDefinition.MediaHierarchyDefinition.Definition.ThrowIfNull();
            dataRequest.FlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ThrowIfNull().IfEmpty();

            var data = dataRequest.PreviousData;

            if (!dataRequest.UseStaticFilter || data is null)
            {
                var dictionaryTables = new List<DataDictionaryTableDetailsDTO>();

                var (startDate, endDate) = await GetDatesAsync(dataRequest.FlowchartDefinition.CalendarDefinition.Definition, dataRequest.OmniClientId, userId, cancellationToken);

                bool showBriefedCTC = dataRequest.ShowBriefedCTC;
                var joins = await GetJoinAsync(startDate, endDate, dataRequest, dictionaryTables, includeSourceMediaToolsData, cancellationToken, showBriefedCTC);

                var isSplitQuery = false;

                var maxConcurrency = Math.Min(Environment.ProcessorCount * 2, joins.Count);
                using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
                var tasks = joins.Select(async query =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var cacheKey = $"{query.GetDeterministicHashCode()}";
                        var result = await cache.GetAsync(
                    $"FlowchartData_{cacheKey}", // maybe something better
                    x => queryLogic.QueryAsync(query, x, isSplitQuery), cancellationToken);
                        return result;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                var results = await Task.WhenAll(tasks);
                var combinedRows = results.SelectMany(c => c.Rows).ToList();
                var columns = results.Select(c => c.ColumnInfo).FirstOrDefault();
                var finalResult = new AthenaQueryResult()
                {
                    ColumnInfo = columns,
                    Rows = combinedRows
                };
                // log.Add(LogLevel.Information, $"QueryLogic.QueryAsync took {sw.ElapsedMilliseconds} ms");
                data = dataConverter.Convert(finalResult);
            }
            return await GetFlowchartData(dataRequest, data, userId, cancellationToken);
        }


        private async Task<ICollection<Dictionary<string, object?>>> GetData(DistinctDataDTO distinctData, bool includeSourceMediaToolsData, string ansid, Guid userId, bool isPreviousLevelDataRequired, CancellationToken cancellationToken)
        {
            var mediaOpsColumnSerachDTO = new MediaopsColumnSearchDTO()
            {
                ClientId = distinctData.OmniClientId.ToString()
            };

            string ansidValue = string.IsNullOrWhiteSpace(ansid) ? GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme) : ansid;

            var activeTemplate = await GetMediaOpsHierarchyDetails(distinctData.OmniClientId, ansidValue, cancellationToken);

            Dictionary<string, int> tableDictionary = new Dictionary<string, int>
            {
                { AthenaConsts.CampaignTable, 0 },
                { AthenaConsts.Channel, 1 },
                { AthenaConsts.Budget, 2 },
                { AthenaConsts.Supplier, 3 },
                { AthenaConsts.Placement, 4 },
            };

            var pmdsVersion = "_v2";
            var table = await GetTableColumnsInfosAsync(distinctData.TableId, distinctData.OmniClientId, cancellationToken);
            var displayNameColumns = activeTemplate?.columns?.Where(c => string.Equals(c.tier, table.Name, StringComparison.CurrentCultureIgnoreCase) && c.isPartOfDisplayName).ToList();
            var parentTable = distinctData.ParentTableId != null ? await GetTableColumnsInfosAsync((Guid)distinctData.ParentTableId, distinctData.OmniClientId, cancellationToken) : null;

            var column = table.Columns.FirstOrDefault(x => x.Name == distinctData.ColumnName);
            var mediaOpsColumnDetails = GetColumnDetails(activeTemplate, table.Name, distinctData.ColumnName);

            var levelTables = distinctData.Levels?.Select(x => GetTableColumnsInfosAsync(x.TableId, distinctData.OmniClientId, cancellationToken).Result).Select(c => new { c.Id, c.Name }).Distinct().ToList();

            var omniClients = await GetOmniClientHierarchy(distinctData.OmniClientId, cancellationToken);

            List<string> selectedTables = new List<string>();
            List<string> auxiliaryJoin = new List<string>();
            List<string> OmniWhere = new List<string>();
            var excludeAuxiliary = new string[] { };

            selectedTables.Add(table.Name);
            if (parentTable != null)
            {
                selectedTables.Add(parentTable.Name);
            }

            if (levelTables != null)
            {
                foreach (var info in levelTables)
                {
                    selectedTables.Add(info.Name);
                }
            }
            var tableDictionaryList = tableDictionary.Select(x => new { x.Key, x.Value }).ToList();

            var distinctTables = selectedTables.Distinct().Select(x => new { Name = x }).ToList();

            bool isSupplierTable = distinctTables.Where(x => x.Name == AthenaConsts.Supplier).Any();

            if (!isSupplierTable)
            {
                distinctTables.Add(new { Name = AthenaConsts.Supplier });
            }

            var tablesWithOrders = (from td in tableDictionaryList
                                    join dt in distinctTables
                                    on td.Key equals dt.Name
                                    select td).ToList();

            var minOrder = tablesWithOrders.Min(x => x.Value);
            var maxOrder = tablesWithOrders.Max(x => x.Value);
            var tableToJoin = string.Empty;
            var tableOrder = tableDictionaryList.Where(y => y.Value >= minOrder && y.Value <= maxOrder).OrderBy(x => x.Value).ToList();

            for (int i = 0; i < tableOrder.Count(); i++)
            {
                var previousTable = i > 0 ? tableOrder[i - 1].Key : string.Empty;
                var currentTable = tableOrder[i].Key;
                if (i == 0)
                {
                    tableToJoin = $" FROM {currentTable}{pmdsVersion} as {currentTable}";
                }
                else
                {
                    tableToJoin += $" INNER JOIN {currentTable}{pmdsVersion} as {currentTable} ON {currentTable}.parent_id = {previousTable}.tier_id AND {currentTable}.{AthenaConsts.OmniGuid} = {previousTable}.{AthenaConsts.OmniGuid} AND {currentTable}.{AthenaConsts.PmdsState} = {previousTable}.{AthenaConsts.PmdsState} AND {currentTable}.{SourceSysMandatoryField} = {previousTable}.{SourceSysMandatoryField}";
                }
                OmniWhere.Add($@"lower({currentTable}.{AthenaConsts.OmniGuid}) IN {omniClients}");
                OmniWhere.Add($"{currentTable}.{AthenaConsts.PmdsState} = 'active'");
                OmniWhere.Add($"lower({currentTable}.{SourceSysMandatoryField} ->> 'data_state') = 'submitted'");

                if (!includeSourceMediaToolsData)
                {
                    OmniWhere.Add($"lower({currentTable}.{AthenaConsts.SourceSys}) <> 'mediatools'");
                }

            }
            string? tableWhere = null;

            var isAuxiliaryColumn = mediaOpsColumnDetails.isAuxiliary;
            bool isParentAuxiliaryColumn = false;
            var nonAuxiliarySelect = !isAuxiliaryColumn ? displayNameColumns?.Count > 0 ? GetConcatenatedColumn(table.Name, column.Name, table.Name + "." + column.Name, column.Name, displayNameColumns) : $"{table.Name}.{column.Name} AS {column.Name}" : "";

            var previousLevelDetails = distinctData.Levels?.LastOrDefault();
            var query = isAuxiliaryColumn ? $"SELECT DISTINCT {GetSelectAuxiliary(table.Name, distinctData.ColumnName, distinctData.ColumnName, false, displayNameColumns, mediaOpsColumnDetails?.isPartOfTierId ?? false)} {tableToJoin}" : $"SELECT DISTINCT {nonAuxiliarySelect} {tableToJoin}";

            if (previousLevelDetails != null && !string.IsNullOrWhiteSpace(previousLevelDetails.ColumnName) && isPreviousLevelDataRequired)
            {
                var levelTable = await GetTableColumnsInfosAsync(previousLevelDetails.TableId, distinctData.OmniClientId, cancellationToken);
                var levelColumn = levelTable.Columns.FirstOrDefault(x => x.Name == previousLevelDetails.ColumnName);
                var previousLevelMediaOpsDetails = GetColumnDetails(activeTemplate, levelTable.Name, previousLevelDetails.ColumnName);
                //var previousLevelTableMediaOpsDetails = GetColumnDetails(activeTemplate, levelTable.Name, previousLevelDetails.ColumnName);
                var isAuxillary = previousLevelMediaOpsDetails.isAuxiliary;
                var displayNameColumnsForPrevious = activeTemplate?.columns?.Where(c => string.Equals(c.tier, levelTable.Name, StringComparison.CurrentCultureIgnoreCase) && c.isPartOfDisplayName).ToList();
                var nonAuxiliarySelectPrevious = !isAuxillary ? displayNameColumnsForPrevious?.Count > 0 ? GetConcatenatedColumn(levelTable.Name, levelColumn.Name, levelTable.Name + "." + levelColumn.Name, levelColumn.Name, displayNameColumnsForPrevious) : $"{levelTable.Name}.{levelColumn.Name} AS {levelColumn.Name}" : "";
                query = isAuxiliaryColumn ? $"SELECT DISTINCT {GetSelectAuxiliary(table.Name, distinctData.ColumnName, distinctData.ColumnName, false, displayNameColumns, mediaOpsColumnDetails?.isPartOfTierId ?? false)}" : $"SELECT DISTINCT {nonAuxiliarySelect}";
                query += isAuxillary ? $",{GetSelectAuxiliary(levelTable.Name, previousLevelDetails.ColumnName, "column2", false, displayNameColumnsForPrevious, previousLevelMediaOpsDetails.isPartOfTierId)}" : $",{nonAuxiliarySelectPrevious}";
                query += $" {tableToJoin}";
            }

            Column parentMediaOpsColumnDetails = null;
            if (parentTable != null)
            {
                var parentTableInfo = parentTable.Id != null ? await GetTableColumnsInfosAsync((Guid)parentTable.Id, distinctData.OmniClientId, cancellationToken) : null;
                var parentcolumnDetails = parentTableInfo.Columns.FirstOrDefault(x => x.Name == distinctData.ParentColumnName);
                parentMediaOpsColumnDetails = GetColumnDetails(activeTemplate, parentTableInfo.Name, distinctData.ParentColumnName);

                isParentAuxiliaryColumn = parentMediaOpsColumnDetails.isAuxiliary;

                if (parentMediaOpsColumnDetails?.isPartOfTierId != null)
                {
                    if (!query.Contains($",jsonb_array_elements({parentTable.Name}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {parentTable.Name}_auxiliary") && isParentAuxiliaryColumn && !parentMediaOpsColumnDetails.isPartOfTierId && parentTable.Name != AthenaConsts.Budget && (parentTable.Name != table.Name || !isAuxiliaryColumn))
                    {
                        query += $",jsonb_array_elements({parentTable.Name}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {parentTable.Name}_auxiliary";
                    }
                }
            }

            if (mediaOpsColumnDetails?.isPartOfTierId != null)
            {
                if (isAuxiliaryColumn && !mediaOpsColumnDetails.isPartOfTierId && !query.Contains($",jsonb_array_elements({table.Name}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {table.Name}_auxiliary"))
                {
                    query += $",jsonb_array_elements({table.Name}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {table.Name}_auxiliary";
                }
            }
            var levelsWhere = string.Empty;

            if (distinctData.Levels?.Count() > 0)
            {
                foreach (var level in distinctData.Levels?.ToList())
                {
                    var levelTable = await GetTableColumnsInfosAsync(level.TableId, distinctData.OmniClientId, cancellationToken);
                    var levelColumn = levelTable.Columns.FirstOrDefault(x => x.Name == level.ColumnName);
                    var isAuxillary = levelColumn == null || levelColumn?.Type == null;
                    var levelMediaOpsColumnDetails = GetColumnDetails(activeTemplate, levelTable.Name, level.ColumnName);
                    if (levelMediaOpsColumnDetails.isAuxiliary)
                    {
                        if (levelMediaOpsColumnDetails != null && !levelMediaOpsColumnDetails.isPartOfTierId)
                        {
                            levelsWhere += $" AND {levelTable.Name}_{AthenaConsts.Auxiliary} ->> '{level.ColumnName}' IN ({string.Join(",", level.SelectedValues.Select(c => SqlQuote(c.Split("~")[0])))})";
                            if (!query.Contains($",jsonb_array_elements({levelTable.Name}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {levelTable.Name}_auxiliary"))
                            {
                                query += $",jsonb_array_elements({levelTable.Name}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {levelTable.Name}_auxiliary";
                            }
                        }
                        else
                        {
                            levelsWhere += $" AND {levelTable.Name}.{AthenaConsts.AuxiliaryPartOfTierId} ->> '{level.ColumnName}' IN ({string.Join(",", level.SelectedValues.Select(c => SqlQuote(c.Split("~")[0])))})";
                        }

                    }
                    else
                    {
                        levelsWhere += $" AND {levelTable.Name}.{levelColumn.Name} IN ({string.Join(",", level.SelectedValues.Select(c => SqlQuote(c.Split("~")[0])))})";
                    }
                }
            }

            var where = GetOmniClientIdRestrictionsBasedOnTablePmds(omniClients, tableWhere ?? table.Name);
            where += isAuxiliaryColumn ? !mediaOpsColumnDetails?.isPartOfTierId ?? false ? $" AND {table.Name}_{AthenaConsts.Auxiliary} ->> '{distinctData.ColumnName}' is not null" : $" AND {table.Name}.{AthenaConsts.AuxiliaryPartOfTierId} ->> '{distinctData.ColumnName}' is not null" : $" AND {table.Name}.{column.Name} is not null";

            if (!string.IsNullOrWhiteSpace(where))
            {
                query += $" WHERE {where}";
            }

            if (parentTable != null)
            {
                var subeLeveleWhere = string.Empty;

                if (isParentAuxiliaryColumn)
                {
                    subeLeveleWhere = GetConstraintAuxiliary(parentTable.Name, distinctData.ParentColumnName, distinctData.ParentSelectedValue.Split('~')[0].Trim(), parentMediaOpsColumnDetails.isPartOfTierId);
                }
                else
                {
                    var parentColumn = parentTable.Columns.FirstOrDefault(x => x.Name == distinctData.ParentColumnName) ?? throw new KeyNotFoundException($"Column {distinctData.ParentColumnName} not found in table {parentTable.Name}.");
                    subeLeveleWhere = GetSubLeveleRestriction(parentTable.Name, parentColumn.Name, distinctData.ParentSelectedValue.Split('~')[0].Trim());
                }

                if (!string.IsNullOrWhiteSpace(subeLeveleWhere))
                {
                    if (!string.IsNullOrWhiteSpace(where))
                    {
                        query += $" AND  {subeLeveleWhere}";
                    }
                    else
                    {
                        query += $" Where  {subeLeveleWhere}";
                    }
                }
            }
            if (!string.IsNullOrEmpty(levelsWhere))
            {
                query += levelsWhere;
            }

            // if query contains jsonb_array_elements({AthenaConsts.Supplier}.auxiliary) as {AthenaConsts.Supplier}_auxiliary any() then we need to add the where clause to the query
            if (!query.Contains($"jsonb_array_elements({AthenaConsts.Supplier}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {AthenaConsts.Supplier}_auxiliary"))
            {
                string insertstring = $", jsonb_array_elements({AthenaConsts.Supplier}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {AthenaConsts.Supplier}_auxiliary ";
                int j = 0;
                string whereClause = " WHERE ";

                j = query.IndexOf(whereClause);
                query = query.Insert(j, insertstring);
            }

            if (distinctData.StartDate != null && distinctData.EndDate != null)
            {
                var isMediaToolClient = activeTemplate?.columns?.Any(x => !string.IsNullOrWhiteSpace(x.sourceSystem) && string.Equals(x.sourceSystem, MediaToolsSource, StringComparison.InvariantCultureIgnoreCase)) ?? false;
                var effectiveDateBasedOnSource = isMediaToolClient ? AthenaConsts.MediaPlansFlightStart : AthenaConsts.MediaPlansEffectiveDate;

                var supplierEffectiveDate = GetAuxiliaryWhereClause(AthenaConsts.Supplier, effectiveDateBasedOnSource);
                OmniWhere.Add($"{supplierEffectiveDate} >= DATE({SqlQuote(distinctData.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty)})");
                OmniWhere.Add($"{supplierEffectiveDate} <= DATE({SqlQuote(distinctData.EndDate?.ToString("yyyy-MM-dd") ?? string.Empty)})");
            }

            query += $" AND {string.Join(" AND ", OmniWhere)}";
            var result = await cache.GetAsync(
                $"DistinctData_{distinctData.TableId}_{distinctData.ColumnName}_{query.GetHashCode()}",
                x => queryLogic.QueryAsync(query, x),
                cancellationToken);

            var data = dataConverter.Convert(result);

            return data;

        }


        /// <summary>
        /// Gets the distinct data asynchronous.
        /// </summary>
        /// <param name="distinctData">The distinct data.</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="ansid">The ansid</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ICollection<object?>> GetDistinctDataAsync(DistinctDataDTO distinctData, bool includeSourceMediaToolsData, string ansid, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting distinct data for user {userId}.");

            var data = await GetData(distinctData, includeSourceMediaToolsData, ansid, userId, false, cancellationToken);

            return data.SelectMany(x => x.Values).Where(x => x != null && !string.IsNullOrWhiteSpace(x.ToString())).ToList();
        }


        /// <summary>
        /// Gets the previous level distinct data asynchronous.
        /// </summary>
        /// <param name="distinctData">The distinct data.</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="ansid">The ansid</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ICollection<DataDTO>> GetPreiousLevelDistinctDataAsync(DistinctDataDTO distinctData, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);

            if (distinctData == null || distinctData.Levels?.Count() == 0)
            {
                return [];
            }
            var data = await GetData(distinctData, includeSourceMediaToolsData, ansidValue, userId, true, cancellationToken);

            return data.Select(x => new DataDTO() { Column1 = Convert.ToString(x.Values.ToList()[0]), Column2 = Convert.ToString(x.Values.ToList()[1]) }).Where(c => !string.IsNullOrWhiteSpace(c.Column1) && !string.IsNullOrWhiteSpace(c.Column2)).ToList();
        }

        /// <summary>
        /// Gets the header data asynchronous.
        /// </summary>
        /// <param name="headerDataRequest">The header data request.</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<HeaderData> GetHeaderDataAsync(HeaderDataRequestDTO headerDataRequest, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting header data for user {userId}.");

            var config = headerDataRequest?.FlowchartDefinition?.HeaderDefinition?.Definition?.Configuration;
            bool hasValueRows = config?.Rows != null && config.Rows.Any(x => x.Type == HeaderRowType.Value);
            bool hasDetails = config?.Details != null;
            if (!hasValueRows && !hasDetails)
            {
                return new HeaderData()
                {
                    Rows = new Collection<HeaderDataRow>()
                };
            }

            ValidateHeaderDataRequest(headerDataRequest);

            var dictionaryTables = new List<DataDictionaryTableDetailsDTO>();
            var (_, restrictions, _) = await GetSelectedColumnsAsync(headerDataRequest.OmniClientId, headerDataRequest.FlowchartDefinition, dictionaryTables, cancellationToken, Enum.Parse<Currency>(Currency.LLL.ToString()));

            var (startDate, endDate) = await GetDatesAsync(headerDataRequest.FlowchartDefinition.CalendarDefinition.Definition, headerDataRequest.OmniClientId, userId, cancellationToken);

            var activeTemplate = await GetMediaOpsHierarchyDetails(headerDataRequest.OmniClientId, GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme), cancellationToken);

            var omniClients = await GetOmniClientHierarchy(headerDataRequest.OmniClientId, cancellationToken);

            var runWhere = await GetRunRestrictionsAsync(headerDataRequest.OmniClientId, headerDataRequest.RunRestrictions, dictionaryTables, null, cancellationToken);

            var allRows = CollectHeaderAndDetailRows(headerDataRequest);
            var tableDetailsTasks = PrefetchTableSchemas(allRows, headerDataRequest.OmniClientId, cancellationToken);
            await Task.WhenAll(tableDetailsTasks.Values);

            var (selectedColumns, columnMeta) = BuildSelectColumns(allRows, tableDetailsTasks, activeTemplate);

            var (tableJoins, auxiliaryJoins, indexBuilder, omniWhere) = BuildTableJoinsAndWhere(
                selectedColumns, runWhere, restrictions, includeSourceMediaToolsData, omniClients);

            var whereClause = BuildWhereClause(
                omniWhere, startDate, endDate, restrictions, runWhere);

            var query = BuildHeaderQuery(selectedColumns, tableJoins, auxiliaryJoins, whereClause);

            log.Add(LogLevel.Information, $"Generated SQL Query for GetHeaderData: {query}");
            var cacheKey = $"HeaderData_{string.Join("_", allRows.Select(r => r.TableId + "_" + r.ColumnName))}_{query.GetDeterministicHashCode()}";
            var result = await cache.GetAsync(cacheKey, x => queryLogic.QueryAsync(query, x), cancellationToken);

            var combinedData = new ReadOnlyCollection<Dictionary<string, object?>>((IList<Dictionary<string, object?>>)dataConverter.Convert(result));

            return MapHeaderData(combinedData, columnMeta, allRows);
        }

        // --- Helper Methods ---

        private void ValidateHeaderDataRequest(HeaderDataRequestDTO request)
        {
            request.FlowchartDefinition.ThrowIfNull();
            request.FlowchartDefinition.HeaderDefinition.ThrowIfNull();
            request.FlowchartDefinition.MediaHierarchyDefinition.ThrowIfNull();
            request.FlowchartDefinition.CalendarDefinition.ThrowIfNull();
        }

        private List<(Guid TableId, string ColumnName, int Order, bool IsDetail)> CollectHeaderAndDetailRows(HeaderDataRequestDTO request)
        {
            var allRows = new List<(Guid TableId, string ColumnName, int Order, bool IsDetail)>(capacity:
                (request.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Rows?.Count ?? 0) +
                (request.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Details?.Rows?.Count ?? 0));

            foreach (var r in request.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Rows)
            {
                if (r.Type == HeaderRowType.Value)
                    allRows.Add((r.TableId!.Value, r.ColumnName!, r.Order, false));
            }

            var detailRows = request.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Details?.Rows;
            if (detailRows != null)
            {
                foreach (var r in detailRows)
                    allRows.Add((r.TableId, r.ColumnName!, r.Order, true));
            }
            return allRows;
        }

        private Dictionary<Guid, Task<DataDictionaryTableDetailsDTO>> PrefetchTableSchemas(List<(Guid TableId, string ColumnName, int Order, bool IsDetail)> allRows, Guid omniClientId, CancellationToken cancellationToken)
        {
            var tableIds = new HashSet<Guid>();
            foreach (var row in allRows)
                tableIds.Add(row.TableId);

            var dict = new Dictionary<Guid, Task<DataDictionaryTableDetailsDTO>>(tableIds.Count);
            foreach (var id in tableIds)
                dict[id] = GetTableColumnsInfosAsync(id, omniClientId, cancellationToken);

            return dict;
        }

        private (List<string> selectedColumns, List<(string Alias, string ColumnName, Guid TableId, int Order, bool IsDetail)> columnMeta) BuildSelectColumns(List<(Guid TableId, string ColumnName, int Order, bool IsDetail)> allRows, Dictionary<Guid, Task<DataDictionaryTableDetailsDTO>> tableDetailsTasks, MediaopsHierarchyDetails activeTemplate)
        {
            var selectedColumns = new List<string>();
            var columnMeta = new List<(string Alias, string ColumnName, Guid TableId, int Order, bool IsDetail)>();
            foreach (var row in allRows)
            {
                var tableDetails = tableDetailsTasks[row.TableId].Result;
                var column = tableDetails.Columns?.FirstOrDefault(x => x.Name == row.ColumnName)
                    ?? new DataDictionaryColumnDetailsDTO { Name = row.ColumnName, Type = ColumnType.String };
                var mediaOpsColumnDetails = GetColumnDetails(activeTemplate, tableDetails.Name, row.ColumnName);
                var columnAlias = $"{HeaderPrefix}_{(row.IsDetail ? "detail_" : "")}{column.Name}";
                string selectCol = mediaOpsColumnDetails?.isAuxiliary == true
                    ? GetSelectAuxiliary(tableDetails.Name, column.Name, columnAlias, false, null, mediaOpsColumnDetails.isPartOfTierId)
                    : $"{tableDetails.Name}.{column.Name} AS {columnAlias}";
                selectedColumns.Add(selectCol);
                columnMeta.Add((columnAlias, column.Name, tableDetails.Id, row.Order, row.IsDetail));
            }
            return (selectedColumns, columnMeta);
        }

        private (List<string> tableJoins, HashSet<string> auxiliaryJoins, StringBuilder indexBuilder, List<string> omniWhere) BuildTableJoinsAndWhere(List<string> selectedColumns, string runWhere, List<string> restrictions, bool includeSourceMediaToolsData, string omniClients)
        {
            if (selectedColumns == null) throw new ArgumentNullException(nameof(selectedColumns));
            if (restrictions == null) throw new ArgumentNullException(nameof(restrictions));
            if (omniClients == null) throw new ArgumentNullException(nameof(omniClients));

            var tableOrder = new[] { AthenaConsts.CampaignTable, AthenaConsts.Channel, AthenaConsts.Budget, AthenaConsts.Supplier, AthenaConsts.Placement };
            var pmdsVersion = "_v2";
            var tableJoins = new List<string>(tableOrder.Length);
            var auxiliaryJoins = new HashSet<string>();
            var indexBuilder = new StringBuilder();
            var omniWhere = new List<string>(tableOrder.Length * 3);

            var tablesNeedingAuxiliary = new HashSet<string>(
                tableOrder.Where(t =>
                    t == AthenaConsts.Supplier ||
                    selectedColumns.Any(col => col.Contains($"{t}_{AthenaConsts.Auxiliary}")) ||
                    (runWhere?.Contains($"{t}_{AthenaConsts.Auxiliary}") ?? false) ||
                    restrictions.Any(res => res.Contains($"{t}_{AthenaConsts.Auxiliary}"))
                )
            );

            for (int i = 0; i < tableOrder.Length; i++)
            {
                var previousTable = i > 0 ? tableOrder[i - 1] : string.Empty;
                var pmdsTable = tableOrder[i];
                if (i == 0)
                {
                    tableJoins.Add($"FROM {pmdsTable}{pmdsVersion} as {pmdsTable}");
                }
                else
                {
                    tableJoins.Add($"INNER JOIN {pmdsTable}{pmdsVersion} as {pmdsTable} ON {pmdsTable}.parent_id = {previousTable}.tier_id AND {pmdsTable}.{AthenaConsts.OmniGuid} = {previousTable}.{AthenaConsts.OmniGuid} AND {pmdsTable}.{AthenaConsts.PmdsState} = {previousTable}.{AthenaConsts.PmdsState} AND {pmdsTable}.{SourceSysMandatoryField} = {previousTable}.{SourceSysMandatoryField}");
                }
                omniWhere.Add($@"lower({pmdsTable}.{AthenaConsts.OmniGuid}) IN {omniClients}");
                omniWhere.Add($"{pmdsTable}.{AthenaConsts.PmdsState} = 'active'");
                omniWhere.Add($"lower({pmdsTable}.{SourceSysMandatoryField} ->> 'data_state') = 'submitted'");

                if (!includeSourceMediaToolsData)
                    omniWhere.Add($"lower({pmdsTable}.{AthenaConsts.SourceSys}) <> 'mediatools'");

                if (tablesNeedingAuxiliary.Contains(pmdsTable))
                {
                    if (pmdsTable == AthenaConsts.Supplier)
                    {
                        indexBuilder.Append($"index_{pmdsTable}");
                        auxiliaryJoins.Add($"jsonb_array_elements({pmdsTable}.{AthenaConsts.AuxiliaryNotPartOfTierId}) WITH ORDINALITY as elements_{pmdsTable}({pmdsTable}_auxiliary, index_{pmdsTable})");
                    }
                    else if (pmdsTable == AthenaConsts.Placement)
                    {
                        indexBuilder.Append($",index_{pmdsTable}");
                        auxiliaryJoins.Add($"jsonb_array_elements({pmdsTable}.{AthenaConsts.AuxiliaryNotPartOfTierId}) WITH ORDINALITY as elements_{pmdsTable}({pmdsTable}_auxiliary, index_{pmdsTable})");
                    }
                    else
                    {
                        auxiliaryJoins.Add($"jsonb_array_elements({pmdsTable}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {pmdsTable}_auxiliary");
                    }
                }
            }

            return (tableJoins, auxiliaryJoins, indexBuilder, omniWhere);
        }

        private StringBuilder BuildWhereClause(List<string> omniWhere, DateTime startDate, DateTime endDate, List<string> restrictions, string runWhere)
        {
            var supplierEffectiveDate = GetAuxiliaryWhereClause(AthenaConsts.Supplier, AthenaConsts.MediaPlansEffectiveDate);
            var whereClause = new StringBuilder();
            whereClause.Append(string.Join(" AND ", omniWhere));
            whereClause.Append($" AND {supplierEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})");
            whereClause.Append($" AND {supplierEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})");
            if (restrictions.Count > 0)
                whereClause.Append($" AND {string.Join(" AND ", restrictions)}");
            if (!string.IsNullOrWhiteSpace(runWhere))
                whereClause.Append($" AND ({runWhere})");
            return whereClause;
        }

        private string BuildHeaderQuery(List<string> selectedColumns, List<string> tableJoins, HashSet<string> auxiliaryJoins, StringBuilder whereClause)
        {
            if (selectedColumns == null) throw new ArgumentNullException(nameof(selectedColumns));
            if (tableJoins == null) throw new ArgumentNullException(nameof(tableJoins));
            if (auxiliaryJoins == null) throw new ArgumentNullException(nameof(auxiliaryJoins));
            if (whereClause == null) throw new ArgumentNullException(nameof(whereClause));

            if (auxiliaryJoins.Count > 0)
            {
                var campaignAux = $"jsonb_array_elements({AthenaConsts.CampaignTable}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {AthenaConsts.CampaignTable}_auxiliary";
                auxiliaryJoins.Add(campaignAux);
                tableJoins.Add($",{string.Join(',', auxiliaryJoins)}");
            }
            return $"SELECT DISTINCT {string.Join(", ", selectedColumns)} {string.Join(" ", tableJoins)} WHERE {whereClause}";
        }

        private HeaderData MapHeaderData(ReadOnlyCollection<Dictionary<string, object?>> combinedData, List<(string Alias, string ColumnName, Guid TableId, int Order, bool IsDetail)> columnMeta, List<(Guid TableId, string ColumnName, int Order, bool IsDetail)> allRows)
        {
            if (combinedData == null) throw new ArgumentNullException(nameof(combinedData));
            if (columnMeta == null) throw new ArgumentNullException(nameof(columnMeta));
            if (allRows == null) throw new ArgumentNullException(nameof(allRows));

            var headerData = new HeaderData { Rows = new Collection<HeaderDataRow>() };
            HeaderDataDetails? details = null;
            if (allRows.Any(r => r.IsDetail))
                details = new HeaderDataDetails { Rows = new Collection<HeaderDataRow>() };

            for (int i = 0; i < columnMeta.Count; i++)
            {
                var meta = columnMeta[i];
                var valueSet = new HashSet<string>();
                foreach (var x in combinedData)
                {
                    if (x != null && x.TryGetValue(meta.Alias, out var value) && value != null)
                    {
                        try
                        {
                            var serialized = JsonSerializer.Serialize(value);
                            valueSet.Add(serialized);
                        }
                        catch (Exception ex)
                        {
                            // Optionally log the error here
                            // log.Add(LogLevel.Error, $"Serialization error for {meta.Alias}", ex);
                        }
                    }
                }
                var values = valueSet.ToList();
                values.Sort(StringComparer.Ordinal);
                var row = new HeaderDataRow
                {
                    ColumnName = meta.ColumnName,
                    TableId = meta.TableId,
                    Order = meta.Order,
                    ValuesJson = values
                };

                if (meta.IsDetail)
                    details?.Rows.Add(row);
                else
                    headerData.Rows.Add(row);
            }

            if (details != null)
                headerData.Details = details;

            return headerData;
        }

        /// <summary>
        /// Gets the summary data asynchronous.
        /// </summary>
        /// <param name="summaryDataRequest">The summary data request.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<SummaryData> GetSummaryDataAsync(SummaryDataRequestDTO summaryDataRequest, Guid userId, CancellationToken cancellationToken)
        {
            // TODO
            throw new NotImplementedException("Coming soon.");
        }

        private void AddGrandTotals(
            FlowchartData result,
            FlowchartDefinition flowchartDefinition,
            List<Dictionary<string, object?>> data)
        {
            var grandTotalDefinition = flowchartDefinition?.GrandTotalDefinition?.Definition;
            grandTotalDefinition?.Selections.ThrowIfNull().IfEmpty();

            var hierarchyLevels = flowchartDefinition!.MediaHierarchyDefinition!.Definition.Levels.OrderBy(x => x.Order).ToList();

            result.GrandTotals = new Collection<GrandTotalData>();

            foreach (var total in grandTotalDefinition.Selections)
            {
                int[] orders = { total.Order };

                result.GrandTotals.Add(new GrandTotalData
                {
                    FlightRange = total.FlightRange,
                    ColumnName = total.ColumnName,
                    TableId = total.TableId,
                    Order = total.Order,
                    Metrics = GetMetrics(total.ColumnName, GrandTotalsPrefix, orders, null, string.Empty, data, null, hierarchyLevels),
                    LocalCurrency = data.GroupBy(x => x[CampaignLocalCurrencyKey]).Select(x => x.Key).FirstOrDefault()?.ToString(),
                });
            }
        }

        private void AddLevel(
           int levelIndex,
           List<int> levelOrder,
           ICollection<FlowchartDataLevel> flowchartDataLevels,
           ICollection<MediaHierarchyLevel> mediaHierarchyLevels,
           TotalsDefinition? totalsDefinition,
           ICollection<Dictionary<string, object?>> bigdata)
        {
            var order = levelOrder[levelIndex];
            var level = mediaHierarchyLevels.First(x => x.Order == order);

            var settings = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order).ToList();

            var data = bigdata.Where(x => x.ContainsKey($"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}")).GroupBy(x => x[$"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}"]);


            var dataLookup = bigdata
              .Where(x => x.ContainsKey($"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}"))
              .ToLookup(x => x[$"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}"]);

            // Use Parallel.ForEach with partitioning for better load balancing
            var processedLevels = new FlowchartDataLevel[settings.Count];

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, settings.Count)
            };

            Parallel.ForEach(
               settings.Select((setting, index) => new { Setting = setting, Index = index }),
               parallelOptions,
               item =>
               {
                   var processedLevel = ProcessSetting(
                       item.Setting,
                       dataLookup,
                       level,
                       levelIndex,
                       levelOrder,
                       mediaHierarchyLevels,
                       totalsDefinition);

                   if (processedLevel != null)
                   {
                       processedLevels[item.Index] = processedLevel;
                   }
               });

            // Add results maintaining order
            foreach (var processedLevel in processedLevels.Where(level => level != null))
            {
                flowchartDataLevels.Add(processedLevel);
            }
        }

        private FlowchartDataLevel? ProcessSetting(MediaHierarchySetting setting,
            ILookup<object?, Dictionary<string, object?>> dataLookup,
            MediaHierarchyLevel level,
            int levelIndex,
            List<int> levelOrder,
            ICollection<MediaHierarchyLevel> mediaHierarchyLevels,
            TotalsDefinition? totalsDefinition
            )
        {
            var settingKey = setting.Name.Split("~")[0];
            var settingData = dataLookup[settingKey].FirstOrDefault();

            if (settingData == null)
            {
                // Try alternative lookup for cases where key formatting might differ
                settingData = dataLookup.SelectMany(g => g).FirstOrDefault(x =>
                {
                    var key = x[$"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}"];
                    var keyStr = key as string;
                    return (!string.IsNullOrEmpty(keyStr) ? keyStr.Split("~")[0] : keyStr) == settingKey;
                });
            }

            if (settingData == null)
                return null;
            var values = settingData.First();

            ICollection<FlowchartDataMetric>? metrics = null;
            ICollection<FlowchartDataSubTotal>? subTotals = null;
            ICollection<FlowchartDataTotal>? totals = null;
            ICollection<FlowchartDataTotal>? actualTotals = null;
            ICollection<FlowchartDataSubTotal>? subTotalSummary = null;
            var list = dataLookup[settingData[$"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}"]].ToList();

            var orders = new int[] { level.Order, setting.Order };
            var isGrpMetric = setting.MetricColumnName == AthenaConsts.GRPs;
            var tasks = new List<Task>();
            Task<ICollection<FlowchartDataMetric>> metricsTask = null;
            Task<ICollection<FlowchartDataTotal>?> actualTotalsTask = null;

            if (levelIndex + 1 >= levelOrder.Count && (setting.SubLevels == null || !setting.SubLevels.Any()))
            {
                if (setting.MetricColumnName != "None")
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    metricsTask = Task.Run(() => GetMetrics(setting.MetricColumnName, LevelMetricPrefix, orders, setting.InflightOverlayColumnName, LevelInflightOverlayPrefix, list, setting.InflightOverlays?.ToList()));
#pragma warning restore CS8604 // Possible null reference argument.
                }
                else
                {
                    metricsTask = Task.Run(() => GetMetrics(DefaultMetricColumnName, LevelMetricPrefix, orders, setting.InflightOverlayColumnName, LevelInflightOverlayPrefix, list, setting.InflightOverlays?.ToList()));
                }
                // on lowest levels
                actualTotalsTask = Task.Run(() => GetTotals(totalsDefinition, list, isGrpMetric));
                tasks.Add(metricsTask);
                tasks.Add(actualTotalsTask);
            }
            // on all the levels
            var totalsTask = Task.Run(() => GetTotals(totalsDefinition, list, isGrpMetric));
            // Subtotals on each level
            var subTotalsTask = Task.Run(() => GetSubTotals(setting.SubTotals, LevelSubTotalPrefix, orders, list));
            var subTotalSummaryTask = Task.Run(() => GetSubTotalSummary(setting.SubTotalSummary, LevelSubTotalSummaryPrefix, orders, list));

            tasks.AddRange([totalsTask, subTotalsTask, subTotalSummaryTask]);
            Task.WaitAll([.. tasks]);

            metrics = metricsTask?.Result;
            actualTotals = actualTotalsTask?.Result;
            totals = totalsTask.Result;
            subTotals = subTotalsTask.Result;
            subTotalSummary = subTotalSummaryTask.Result;
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8629 // Nullable value type may be null.
            var newLevel = new FlowchartDataLevel
            {
                TableId = level.TableId,
                ColumnName = level.ColumnName,
                Name = setting.Name,
                Order = setting.Order,
                Metrics = metrics,
                SubTotals = subTotals,
                Totals = totals,
                FlightRange = setting.FlightRange,
                MetricColumnName = setting.MetricColumnName,
                MetricTableId = (Guid)setting.MetricTableId,
                ActualTotals = actualTotals,
                BriefedCtcTotals = level.ColumnName == AthenaConsts.Channel ? GetBriefedCtcTotals(list) : new List<FlowchartDataTotal>(),
                LocalCurrency = list.GroupBy(x => x[CampaignLocalCurrencyKey]).Select(x => x.Key).FirstOrDefault()?.ToString(),
                SubTotalSummary = subTotalSummary,
            };
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning restore CS8601 // Possible null reference assignment.

            if (setting.SubLevels != null)
            {
                var sublevels = setting.SubLevels.OrderBy(x => x.Order).Select(x => x.Order).ToList();

                newLevel.SubLevels ??= new Collection<FlowchartDataSubLevel>();

                AddSubLevel(0, sublevels, level, setting, setting.SubLevels, totalsDefinition, list, newLevel.SubLevels);
            }

            if (levelIndex + 1 < levelOrder.Count)
            {
                newLevel.Levels = new Collection<FlowchartDataLevel>();

                AddLevel(levelIndex + 1, levelOrder, newLevel.Levels, mediaHierarchyLevels, totalsDefinition, list);
            }

            return newLevel;

        }

        private void AddSubLevel(
            int levelIndex, List<int> levelOrder,
            MediaHierarchyLevel level,
            MediaHierarchySetting parentSetting,
            ICollection<MediaHierarchySubLevel> subLevels,
            TotalsDefinition? totalsDefinition,
            List<Dictionary<string, object?>> bigdata,
            ICollection<FlowchartDataSubLevel> newLevels)
        {
            var order = levelOrder[levelIndex];
            var sublevel = subLevels.First(x => x.Order == order);

            var data = bigdata.GroupBy(x => x[$"{MediaHierarchySubLevelPrefix}_{level.Order}_{parentSetting.Order}_{sublevel.Order}_{sublevel.ColumnName}"]);

            foreach (var setting in sublevel.Settings.Where(x => x.Enabled).OrderBy(x => x.Order))
            {
                var settingData = data.FirstOrDefault(x => (!string.IsNullOrEmpty(x.Key as string) ? (x.Key as string).Split("~")[0] : (x.Key as string)) == setting.Name.Split("~")[0]);

                if (settingData != null)
                {
                    var values = settingData.First();

                    ICollection<FlowchartDataMetric>? metrics = null;
                    ICollection<FlowchartDataSubTotal>? subTotals = null;
                    ICollection<FlowchartDataTotal>? totals = null;

                    if (levelIndex + 1 >= levelOrder.Count)
                    {
                        var isGrpMetric = setting.MetricColumnName == AthenaConsts.GRPs;
                        // on the lowest level only
                        var list = settingData.ToList();

                        var orders = new int[] { level.Order, parentSetting.Order, sublevel.Order, setting.Order };

                        if (setting.MetricColumnName != "None")
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            metrics = GetMetrics(setting.MetricColumnName, SubLevelMetricPrefix, orders, setting.InflightOverlayColumnName, SubLevelInflightOverlayPrefix, list);
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                        else
                        {
                            metrics = GetMetrics(DefaultMetricColumnName, SubLevelMetricPrefix, orders, setting.InflightOverlayColumnName, SubLevelInflightOverlayPrefix, list);
                        }
                        subTotals = GetSubTotals(setting.SubTotals, SubLevelSubTotalPrefix, orders, list);
                        totals = GetTotals(totalsDefinition, list, isGrpMetric);
                    }

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8629 // Nullable value type may be null.
                    var newSub = new FlowchartDataSubLevel
                    {
                        ColumnName = sublevel.ColumnName,
                        Order = setting.Order,
                        TableId = sublevel.TableId,
                        Totals = totals,
                        SubTotals = subTotals,
                        Name = setting.Name,
                        Metrics = metrics,
                        FlightRange = setting.FlightRange,
                        MetricColumnName = setting.MetricColumnName,
                        MetricTableId = (Guid)setting.MetricTableId,
                        BriefedCtcTotals = sublevel.ColumnName == AthenaConsts.Channel ? GetBriefedCtcTotals(settingData.ToList()) : new List<FlowchartDataTotal>(),
                    };
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning restore CS8601 // Possible null reference assignment.

                    newLevels.Add(newSub);

                    if (levelIndex + 1 < levelOrder.Count)
                    {
                        newSub.SubLevels = new Collection<FlowchartDataSubLevel>();

                        AddSubLevel(levelIndex + 1, levelOrder, level, parentSetting, subLevels, totalsDefinition, settingData.ToList(), newSub.SubLevels);
                    }
                }
            }
        }

        private async Task<(DateTime StartDate, DateTime EndDate)> GetBroadcastCalendarDatesAsync(int year, Guid userId, CancellationToken cancellationToken)
        {
            var broadcastCalendar = await GetBroadcastCalendarAsync(year, userId, cancellationToken);

            var startDate = broadcastCalendar.SelectMany(x => x).Where(x => x.ColumnName == AthenaConsts.RollPeriodFirstDay)
                            .Select(x => Json.Deserialize<DateTime>(x.JsonValue)).Min();

            var endDate = broadcastCalendar.SelectMany(x => x).Where(x => x.ColumnName == AthenaConsts.RollPeriodLastDay)
                            .Select(x => Json.Deserialize<DateTime>(x.JsonValue)).Max();

            return (startDate, endDate);
        }

        private async Task<(DateTime StartDate, DateTime EndDate)> GetClientCalendarDatesAsync(int year, Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            var clientCalendar = await GetClientCalendarAsync(year.ToString(), omniClientId, userId, cancellationToken);

            var select = clientCalendar.SelectMany(x => x).Where(x => x.ColumnName == AthenaConsts.ClientRollPeriodClientWeekDayDate)
                            .Select(x => Json.Deserialize<DateTime>(x.JsonValue));

            return (select.Min(), select.Max());
        }

        private async Task<(DateTime StartDate, DateTime EndDate)> GetDatesAsync(
            CalendarDefinition calendarDefinition,
            Guid omniClientId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            DateTime startDate;
            DateTime endDate;

            if (!calendarDefinition.Configuration.IsReportingTimeFrame)
            {
                calendarDefinition.Configuration.CustomStartDate.ThrowIfNull();
                calendarDefinition.Configuration.CustomEndDate.ThrowIfNull();

                startDate = calendarDefinition.Configuration.CustomStartDate.Value;
                endDate = calendarDefinition.Configuration.CustomEndDate.Value;
            }
            else
            {
                calendarDefinition.Configuration.ReportingTimeFrame.ThrowIfNull();

                (startDate, endDate) = calendarDefinition.Configuration.ReportingTimeFrame.Value switch
                {
                    CalendarReportingTimeFrame.CurrentYear => calendarDefinition.Configuration.Type switch
                    {
                        CalendarType.Broadcast => await GetBroadcastCalendarDatesAsync(DateTime.UtcNow.Year, userId, cancellationToken),
                        CalendarType.Client => await GetClientCalendarDatesAsync(DateTime.UtcNow.Year, omniClientId, userId, cancellationToken),
                        CalendarType.Standard => (new DateTime(DateTime.UtcNow.Year, 1, 1), new DateTime(DateTime.UtcNow.Year, 12, 31)),
                        _ => throw new ArgumentException($"Calendar type {calendarDefinition.Configuration.Type} is not supported."),
                    },
                    CalendarReportingTimeFrame.LastYear => calendarDefinition.Configuration.Type switch
                    {
                        CalendarType.Broadcast => await GetBroadcastCalendarDatesAsync(DateTime.UtcNow.AddYears(-1).Year, userId, cancellationToken),
                        CalendarType.Client => await GetClientCalendarDatesAsync(DateTime.UtcNow.AddYears(-1).Year, omniClientId, userId, cancellationToken),
                        CalendarType.Standard => (new DateTime(DateTime.UtcNow.AddYears(-1).Year, 1, 1), new DateTime(DateTime.UtcNow.AddYears(-1).Year, 12, 31)),
                        _ => throw new ArgumentException($"Calendar type {calendarDefinition.Configuration.Type} is not supported."),
                    },
                    _ => throw new NotSupportedException($"Reporting time frame {calendarDefinition.Configuration.ReportingTimeFrame} is not supported."),
                };
            }

            return (startDate, endDate);
        }

        private async Task<FlowchartData> GetFlowchartData(
             DataRequestDTO dataRequest,
             ICollection<Dictionary<string, object?>> data, Guid userId, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = new FlowchartData()
            {
                Levels = new Collection<FlowchartDataLevel>(),
                RawData = data,
            };

            // Apply static filters if enabled
            ICollection<Dictionary<string, object?>> filteredData = data;
            if (dataRequest.UseStaticFilter)
            {
                var (startDate, endDate) = await GetDatesAsync(
                    dataRequest.FlowchartDefinition.CalendarDefinition.Definition,
                    dataRequest.OmniClientId, userId, cancellationToken);

                filteredData = FilterBasedOnCalendar(filteredData, startDate, endDate);
                filteredData = FilterBasedOnRestrictions(dataRequest, filteredData);
                filteredData = FilterBasedOnMediaHierarchy(dataRequest, filteredData);
            }

            var flowchartDefinition = dataRequest.FlowchartDefinition;
            var levels = flowchartDefinition!.MediaHierarchyDefinition!.Definition.Levels.OrderBy(x => x.Order).Select(x => x.Order).ToList();
            var tasks = new List<Task>();

            var levelsTask = Task.Run(() =>
            {
                var tempLevels = new Collection<FlowchartDataLevel>();
                AddLevel(0, levels, tempLevels, flowchartDefinition!.MediaHierarchyDefinition!.Definition.Levels, flowchartDefinition.TotalsDefinition?.Definition, filteredData);
                return tempLevels;
            });
            tasks.Add(levelsTask);
            // Fix for CS8619: Ensure the Task return type matches the nullable reference type

            if (flowchartDefinition.GrandTotalDefinition?.Definition != null)
            {
                AddGrandTotals(result, flowchartDefinition, filteredData.ToList());
                Task.WaitAll(tasks.ToArray());
                result.Levels = levelsTask.Result;
            }
            else
            {
                Task.WaitAll(tasks.ToArray());

                result.Levels = levelsTask.Result;
            }

            sw.Stop();
            log.Add(LogLevel.Information, $"GetFlowchartData took {sw.ElapsedMilliseconds} ms");
            return result;
        }

        private ICollection<Dictionary<string, object?>> FilterBasedOnCalendar(
                    ICollection<Dictionary<string, object?>> data,
                    DateTime startDate,
                    DateTime endDate)
        {
            if (data is null || data.Count == 0)
            {
                return Array.Empty<Dictionary<string, object?>>();
            }

            // Use a LINQ query for a more concise and readable filter.
            return data.AsParallel()
                .Where(row =>
                {
                    if (row is null || !row.TryGetValue(EffectiveDateKey, out var value) || value is null)
                        return false;

                    if (value is DateTime date)
                        return date >= startDate && date <= endDate;

                    return DateTime.TryParse(value.ToString(), out date) && date >= startDate && date <= endDate;
                })
                .ToList();
        }

        /// <summary>
        /// Filters data based on media hierarchy using pre-compiled regex and O(1) lookup.
        /// </summary>
        private ICollection<Dictionary<string, object?>> FilterBasedOnMediaHierarchy(
            DataRequestDTO dataRequest,
            ICollection<Dictionary<string, object?>> data)
        {
            var levels = dataRequest.FlowchartDefinition?.MediaHierarchyDefinition?.Definition?.Levels;
            if (levels == null || !levels.Any() || data == null || data.Count == 0)
                return data;

            // Build lookup dictionary for selected values per level
            var levelValueDict = levels
                .Where(level => level.Settings != null && level.Settings.Any(s => s.Enabled) && !string.IsNullOrWhiteSpace(level.ColumnName))
                .ToDictionary(
                    level => $"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}",
                    level => level.Settings.Where(s => s.Enabled)
                        .Select(s => s.Name?.Split('~')[0])
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .ToHashSet()
                );

            // Use TryGetValue for O(1) lookup, avoid repeated LINQ
            return data.AsParallel().Where(row =>
            {
                if (row == null) return false;
                foreach (var kvp in levelValueDict)
                {
                    if (!row.TryGetValue(kvp.Key, out var value) || value == null || !kvp.Value.Contains(value.ToString()))
                        return false;
                }
                return true;
            }).ToList();
        }

        private ICollection<Dictionary<string, object?>> FilterBasedOnRestrictions(
             DataRequestDTO dataRequest,
             ICollection<Dictionary<string, object?>> data)
        {
            var restrictions = dataRequest?.RunRestrictions;
            if (restrictions == null || restrictions.Count == 0 || data == null || data.Count == 0)
            {
                return data;
            }

            var restrictionGroups = restrictions
                .GroupBy(r => new { r.TableId, r.ColumnName })
                .Select(group =>
                {
                    var filterValues = group
                        .Select(r => JsonSerializer.Deserialize<string>(r.ValueJson, GetJsonSerializerOptions()))
                        .ToHashSet();

                    if (filterValues.Count == 0)
                    {
                        return null;
                    }

                    var keyRegex = new Regex($@"^{MediaHierarchyLevelPrefix}\d+_{Regex.Escape(group.Key.ColumnName)}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    return new { Regex = keyRegex, Values = filterValues };
                })
                .Where(g => g != null)
                .ToList();

            if (restrictionGroups.Count == 0)
            {
                return data;
            }

            return data.AsParallel().Where(row =>
            {
                if (row == null) return false;

                return restrictionGroups.All(group =>
                    row.Any(kv =>
                        kv.Key != null &&
                        kv.Value != null &&
                        group.Regex.IsMatch(kv.Key) &&
                        group.Values.Contains(kv.Value.ToString())
                    )
                );
            }).ToList();
        }

        private async Task<List<string>> GetJoinAsync(
            DateTime startDate, DateTime endDate,
            DataRequestDTO dataRequest,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            bool includeSourceMediaToolsData,
            CancellationToken cancellationToken,
            bool showBriefedCTC)
        {
            var whereBuilder = new StringBuilder();
            var selectedColumnsBuilder = new StringBuilder();
            var tableToJoinBuilder = new StringBuilder();
            var indexBuilder = new StringBuilder();
            var auxiliaryJoin = new Dictionary<string, string>();
            var omniWhere = new List<string>();

            bool isLocalCurrency = dataRequest.Currency == Currency.LLL.ToString();
            var ANsid = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var activeTemplate = await GetMediaOpsHierarchyDetails(dataRequest.OmniClientId, ANsid, cancellationToken);
            var isMediaToolClient = activeTemplate?.columns?.Any(x => !string.IsNullOrWhiteSpace(x.sourceSystem) && string.Equals(x.sourceSystem, MediaToolsSource, StringComparison.InvariantCultureIgnoreCase)) ?? false;
            var client = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(
                dataRequest.OmniClientId, cancellationToken);
            // Legend code
            var legendColumn = dataRequest.FlowchartDefinition?.ThemeDefinition?.Definition?.LegendTheme?.ColumnName;
            var otherSelected = new Dictionary<string, List<string>>();
            var otherWhere = new Dictionary<string, List<string>>();
            if (!string.IsNullOrWhiteSpace(legendColumn))
            {
                var (themeTable, _, isAuxillaryTheme) = await GetTableColumnAsync(
                    (Guid)dataRequest.FlowchartDefinition?.ThemeDefinition?.Definition?.LegendTheme?.TableId,
                    dataRequest.OmniClientId, legendColumn, dictionaryTables, cancellationToken);
                var mediaOpsTableDetails = GetColumnDetails(activeTemplate, themeTable.Name, legendColumn);
                if (mediaOpsTableDetails.isAuxiliary)
                {
                    AddTodDictionary(otherSelected, themeTable.Name, GetSelectAuxiliary(themeTable.Name, legendColumn, $"{LegendPrefix}", false, null, mediaOpsTableDetails.isPartOfTierId), mediaOpsTableDetails);
                }
                else
                {
                    AddTodDictionary(otherSelected, themeTable.Name, $"{themeTable.Name}.{legendColumn} AS {LegendPrefix}");
                }
            }

            var runWhere = await GetDirectRunRestrictionsAsync(
                dataRequest.OmniClientId, dataRequest.RunRestrictions, dictionaryTables, activeTemplate, cancellationToken);
            var sw = new Stopwatch();
            sw.Start();
            var (selected, restrictions) = await GetDirectSelectedColumnsAsync(
                dataRequest.OmniClientId, dataRequest.FlowchartDefinition, dictionaryTables, cancellationToken,
                Enum.Parse<Currency>(dataRequest.Currency ?? Currency.LLL.ToString()), activeTemplate);

            sw.Stop();
            log.Add(LogLevel.Information, $"GetSelectedColumnsAsync took {sw.ElapsedMilliseconds} ms");
            var distinctSelectedColumns = new HashSet<string>(StringComparer.Ordinal);

            foreach (var y in selected)
            {
                var newValues = new List<string>();
                foreach (var x in y.Value)
                {
                    var replaced = LevelMetricPatternForSelect().Replace(x, $"{GenericMetricPrefix}_$2");
                    replaced = LevelSubTotalMetricPatternForSelect().Replace(replaced, $"{GenericMetricPrefix}_$2");
                    replaced = GrandTotalMetricPatternForSelect().Replace(replaced, $"{GenericMetricPrefix}_$2");
                    replaced = LevelSubTotalSummaryMetricPatternForSelect().Replace(replaced, $"{GenericMetricPrefix}_$2");
                    replaced = TotalsMetricPatternForSelect().Replace(replaced, $"{GenericMetricPrefix}_$2");
                    replaced = InflightRegex().Replace(replaced, $"{GenericInflightOverlayPrefix}_$1");
                    newValues.Add(replaced);
                    distinctSelectedColumns.Add(replaced);
                }
                y.Value.Clear();
                y.Value.AddRange(newValues.Distinct());
            }

            selectedColumnsBuilder.AppendJoin(", ", distinctSelectedColumns);

            var tableOrder = new[] { AthenaConsts.CampaignTable, AthenaConsts.Channel, AthenaConsts.Budget, AthenaConsts.Supplier, AthenaConsts.Placement };
            var excludeAuxiliary = Array.Empty<string>();
            var pmdsVersion = "_v2";
            var omniClients = await GetOmniClientHierarchy(dataRequest.OmniClientId, cancellationToken);

            for (int i = 0; i < tableOrder.Length; i++)
            {
                var previousTable = i > 0 ? tableOrder[i - 1] : string.Empty;
                var table = tableOrder[i];
                if (i == 0)
                {
                    tableToJoinBuilder.Append($" FROM {table}{pmdsVersion} as {table}");
                }
                else
                {
                    tableToJoinBuilder.Append($" INNER JOIN {table}{pmdsVersion} as {table} ON {table}.parent_id = {previousTable}.tier_id")
                        .Append($" AND {table}.{AthenaConsts.OmniGuid} = {previousTable}.{AthenaConsts.OmniGuid}")
                        .Append($" AND {table}.{AthenaConsts.PmdsState} = {previousTable}.{AthenaConsts.PmdsState}")
                        .Append($" AND {table}.{SourceSysMandatoryField} = {previousTable}.{SourceSysMandatoryField}");
                }
                AddTodDictionary(otherWhere, table, $@"lower({table}.{AthenaConsts.OmniGuid}) IN {omniClients}");
                AddTodDictionary(otherWhere, table, $"{table}.{AthenaConsts.PmdsState} = 'active'");
                AddTodDictionary(otherWhere, table, $"lower({table}.{SourceSysMandatoryField} ->> 'data_state') = 'submitted'");

                if (!includeSourceMediaToolsData)
                {
                    AddTodDictionary(otherWhere, table, $"lower({table}.{AthenaConsts.SourceSys}) <> 'mediatools'");
                }

                if (!excludeAuxiliary.Contains(table) &&
                    (table == AthenaConsts.Supplier ||
                     selectedColumnsBuilder.ToString().Contains($"{table}_{AthenaConsts.Auxiliary}") ||
                     runWhere.Any(c => c.Value.Any(d => d.Contains($"{table}_{AthenaConsts.Auxiliary}")))))
                {
                    if (table == AthenaConsts.Supplier)
                    {
                        indexBuilder.Append($"index_{table}");
                        auxiliaryJoin.Add($"{table}_auxiliary", $"jsonb_array_elements({table}.{AthenaConsts.AuxiliaryNotPartOfTierId}) WITH ORDINALITY as elements_{table}({table}_auxiliary, index_{table})");
                    }
                    else if (table == AthenaConsts.Placement)
                    {
                        indexBuilder.Append($",index_{table}");
                        auxiliaryJoin.Add($"{table}_auxiliary", $"jsonb_array_elements({table}.{AthenaConsts.AuxiliaryNotPartOfTierId}) WITH ORDINALITY as elements_{table}({table}_auxiliary, index_{table})");
                    }
                    else
                    {
                        auxiliaryJoin.Add($"{table}_auxiliary", $"jsonb_array_elements({table}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {table}_auxiliary");
                    }
                }
            }
            var campaignAux = $"jsonb_array_elements({AthenaConsts.CampaignTable}.{AthenaConsts.AuxiliaryNotPartOfTierId}) as {AthenaConsts.CampaignTable}_auxiliary";
            if (!auxiliaryJoin.Keys.Contains($"{AthenaConsts.CampaignTable}_auxiliary"))
            {
                auxiliaryJoin.Add($"{AthenaConsts.CampaignTable}_auxiliary", campaignAux);
            }

            string effectiveDateColumnBasedOnSource = isMediaToolClient ? AthenaConsts.MediaPlansFlightStart : AthenaConsts.MediaPlansEffectiveDate;
            var useParentFlightStart = client?.UseParentFlightDates ?? false;
            var tempMediaOpsDetails = new Column() { isAuxiliary = true, isPartOfTierId = false };
            AddTodDictionary(otherSelected, AthenaConsts.Supplier, $"{AthenaConsts.Supplier}_{AthenaConsts.Auxiliary} ->> '{effectiveDateColumnBasedOnSource}' as {EffectiveDateKey}", tempMediaOpsDetails);
            AddTodDictionary(otherSelected, AthenaConsts.CampaignTable, $"{AthenaConsts.CampaignTable}_{AthenaConsts.Auxiliary} ->> 'local_currency_name' as {CampaignLocalCurrencyKey}", tempMediaOpsDetails);

            BuildFlightDatesSelection(otherSelected, useParentFlightStart);

            var queryBuilder = new StringBuilder();

            var supplierEffectiveDate = GetAuxiliaryWhereClause(AthenaConsts.Supplier, effectiveDateColumnBasedOnSource);

            AddTodDictionary(otherWhere, AthenaConsts.Supplier, $"{supplierEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})", tempMediaOpsDetails);
            AddTodDictionary(otherWhere, AthenaConsts.Supplier, $"{supplierEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})", tempMediaOpsDetails);

            if (restrictions?.Count > 0)
            {
                whereBuilder.Append($" AND {string.Join(" AND ", restrictions)}");
            }

            var finalSelect = otherSelected.Concat(selected).GroupBy(c => c.Key).ToDictionary(g => g.Key, g => g.SelectMany(v => v.Value).ToList());
            var finalWhere = otherWhere.Concat(restrictions).Concat(runWhere).GroupBy(c => c.Key).ToDictionary(g => g.Key, g => g.SelectMany(v => v.Value).ToList());
            queryBuilder.Append(QueryBuilder.Build(tableOrder, client?.Version ?? 2, auxiliaryJoin, finalSelect, finalWhere));

            var queries = new List<string>([queryBuilder.ToString()]);

            // Handle briefedCtcExists
            var selectString = string.Join(", ", selected.Values.SelectMany(v => v));
            bool briefedCtcExists = selectString.Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.BriefedCtc}_local')::float AS") ||
                                    selectString.Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.BriefedCtc}')::float AS") ||
                                    selectString.Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.Budget}_local')::float AS") ||
                                    selectString.Contains($"({AthenaConsts.Budget}_{AthenaConsts.Auxiliary}->>'{AthenaConsts.Budget}')::float AS");

            if (briefedCtcExists)
            {
                queries.Add(QueryBuilder.Build(tableOrder, client?.Version ?? 2, auxiliaryJoin, finalSelect, finalWhere, true));
            }

            log.Add(LogLevel.Information, $"query getdataasync {queryBuilder}");
            return queries;
        }

        /// <summary>
        ///  Selects the flight range from the auxiliary from mygrid flight start or flight start dates.
        /// </summary>
        /// <param name="select">select</param>
        /// <param name="useParentFlightDates">useParentFlightDates</param>
        private void BuildFlightDatesSelection(Dictionary<string, List<string>> select, bool useParentFlightDates)
        {
            var tempMediaOpsDetails = new Column() { isAuxiliary = true, isPartOfTierId = false };
            if (!useParentFlightDates)
            {
                AddTodDictionary(select, AthenaConsts.Supplier, $"{AthenaConsts.Supplier}_{AthenaConsts.Auxiliary} ->> '{AthenaConsts.MediaPlansFlightStart}' as {FlightStartKey}", tempMediaOpsDetails);
                AddTodDictionary(select, AthenaConsts.Supplier, $"{AthenaConsts.Supplier}_{AthenaConsts.Auxiliary} ->> '{AthenaConsts.MediaPlansFlightEnd}' as {FlightEndKey}", tempMediaOpsDetails);
            }
            else
            {
                AddTodDictionary(select, AthenaConsts.Supplier, $"{AthenaConsts.Supplier}_{AthenaConsts.Auxiliary} ->> '{AthenaConsts.FlightStart}' as {FlightStartKey}", tempMediaOpsDetails);
                AddTodDictionary(select, AthenaConsts.Supplier, $"{AthenaConsts.Supplier}_{AthenaConsts.Auxiliary} ->> '{AthenaConsts.FlightEnd}' as {FlightEndKey}", tempMediaOpsDetails);
            }
        }

        private string BuildCompositeKey(Dictionary<string, object?> row, IEnumerable<MediaHierarchyLevel> hierarchyLevels)
        {
            var keyParts = new List<string>();
            foreach (var level in hierarchyLevels)
            {
                var key = $"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}";
                var value = row.ContainsKey(key) ? row[key]?.ToString() : "";
                keyParts.Add($"{level.ColumnName}:{value}");
            }
            return string.Join(",", keyParts);
        }

        private ICollection<FlowchartDataMetric> GetMetrics(
    string metricColumnName, string metricPrefix,
    int[]? orders,
    string? inflightOverlayColumnName, string inflightOverlayPrefix,
    List<Dictionary<string, object?>> data, List<MediaHierarchyInflightOverlay>? mediahierarchyInflightOverlays = null, List<MediaHierarchyLevel>? hierarchyLevels = null)
        {
            var additional = orders == null ? string.Empty : $"_{string.Join("_", orders)}";
            var metricField = $"{metricPrefix}{additional}_{metricColumnName}";
            var baseMetricField = $"{GenericMetricPrefix}_{metricColumnName}";

            // For small datasets, process sequentially to avoid parallelization overhead.
            if (data.Count < 200)
            {
                var sequentialResult = new List<FlowchartDataMetric>(data.Count);
                foreach (var row in data)
                {
                    if ((row.ContainsKey(metricField) && row[metricField] != null) ||
                        (row.ContainsKey(baseMetricField) && row[baseMetricField] != null))
                    {
                        sequentialResult.Add(CreateFlowchartDataMetric(row, metricField, baseMetricField, mediahierarchyInflightOverlays, hierarchyLevels));
                    }
                }
                return sequentialResult;
            }

            // For larger datasets, use PLINQ for parallel processing.
            var parallelResult = data.AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Where(row => (row.ContainsKey(metricField) && row[metricField] != null) ||
                             (row.ContainsKey(baseMetricField) && row[baseMetricField] != null))
                .Select(row => CreateFlowchartDataMetric(row, metricField, baseMetricField, mediahierarchyInflightOverlays, hierarchyLevels))
                .ToList();

            return parallelResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FlowchartDataMetric CreateFlowchartDataMetric(
            Dictionary<string, object?> row,
            string metricField,
            string baseMetricField,
            List<MediaHierarchyInflightOverlay>? mediahierarchyInflightOverlays,
            List<MediaHierarchyLevel>? hierarchyLevels)
        {
            var hasActualMetric = row.TryGetValue(metricField, out var metricValue) && metricValue != null;
            var valueJson = hasActualMetric
                ? Json.Serialize(metricValue)
                : Json.Serialize(row[baseMetricField]);

            row.TryGetValue(EffectiveDateKey, out var effectiveDateObj);
            row.TryGetValue(FlightEndKey, out var flightEndObj);
            row.TryGetValue(FlightStartKey, out var flightStartObj);

            var effectiveDate = effectiveDateObj != null ? DateOnly.FromDateTime(Convert.ToDateTime(effectiveDateObj)) : DateOnly.MaxValue;
            var flightEnd = flightEndObj != null ? DateOnly.FromDateTime(Convert.ToDateTime(flightEndObj)) : DateOnly.MaxValue;
            var flightStart = flightStartObj != null ? DateOnly.FromDateTime(Convert.ToDateTime(flightStartObj)) : DateOnly.MinValue;

            var inflightOverlays = mediahierarchyInflightOverlays?.Count > 0
                ? ProcessInflightOverlays(mediahierarchyInflightOverlays, row, flightStart, flightEnd)
                : new List<string>();

            var legendJson = row.TryGetValue(LegendPrefix, out var legend) && legend != null
                ? Json.Serialize(legend)
                : null;

            var compositeKey = hierarchyLevels != null
                ? BuildCompositeKey(row, hierarchyLevels)
                : null;

            return new FlowchartDataMetric
            {
                EffectiveDate = effectiveDate,
                FlightEnd = flightEnd,
                FlightStart = flightStart,
                ValueJson = valueJson,
                InflightOverlays = inflightOverlays,
                Legend = legendJson,
                CompositeKey = compositeKey,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<string> ProcessInflightOverlays(
            List<MediaHierarchyInflightOverlay> overlays,
            Dictionary<string, object?> row,
            DateOnly flightStart,
            DateOnly flightEnd)
        {
            var result = new List<string>(overlays.Count);
            foreach (var overlay in overlays)
            {
                if (overlay.ColumnName == CustomFlightRange)
                {
                    result.Add(Json.Serialize($"{flightStart.ToString(DateFormat)} - {flightEnd.ToString(DateFormat)}"));
                }
                else
                {
                    var overlayColumn = $"{GenericInflightOverlayPrefix}_{overlay.ColumnName}";
                    var value = row.TryGetValue(overlayColumn, out var overlayValue) ? overlayValue : null;
                    result.Add(Json.Serialize(value));
                }
            }
            return result;
        }

        private string GetSubLeveleRestriction(string? tableName, string? columnName, string? value)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return string.Empty;
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                return $"{tableName}.{columnName}={SqlQuote(Convert.ToString(value))}";
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private async Task<string> GetRunRestrictionsAsync(
            Guid omniClientId,
            ICollection<RunRestriction> restrictions,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            MediaopsHierarchyDetails activeTemplate,
            CancellationToken cancellationToken)
        {
            var result = string.Empty;

            // group by tableid and column to format the query with multiple conditions
            var restrictionGroup = (from r in restrictions
                                    group r by new { r.TableId, r.ColumnName } into groups
                                    select new
                                    {
                                        TableId = groups.Key,
                                        Items = groups.Select(c => c),
                                    }).ToList();

            foreach (var restrictionsGroup in restrictionGroup)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " AND ";
                }
                var subResult = string.Empty;

                foreach (var restriction in restrictionsGroup.Items)
                {
                    var (table, column, isAuxillary) = await GetTableColumnAsync(restriction.TableId, omniClientId, restriction.ColumnName, dictionaryTables, cancellationToken);
                    var mediaOpsTableColumnDetails = GetColumnDetails(activeTemplate, table.Name, restriction.ColumnName);
                    if (!string.IsNullOrEmpty(subResult))
                    {
                        subResult += " OR ";
                    }

                    var value = column.Type switch
                    {
                        ColumnType.Boolean => SqlQuote(Json.Deserialize<bool>(restriction.ValueJson).ToString()),
                        ColumnType.Date => $"DATE({SqlQuote(Json.Deserialize<DateTime>(restriction.ValueJson).ToString("yyyy-MM-dd"))})",
                        ColumnType.Decimal => SqlQuote(Json.Deserialize<decimal>(restriction.ValueJson).ToString()!.Replace(",", ".")),
                        ColumnType.LongInteger => SqlQuote(Json.Deserialize<long>(restriction.ValueJson).ToString()),
                        ColumnType.String => SqlQuote(Json.Deserialize<string>(restriction.ValueJson)!),
                        ColumnType.Jsonb => SqlQuote(Json.Deserialize<string>(restriction.ValueJson)!),
                        _ => throw new NotSupportedException($"Column type {column.Type} is not supported."),
                    };
                    if (mediaOpsTableColumnDetails.isAuxiliary)
                    {
                        subResult += GetRunConstraintAuxiliary(table.Name, column.Name, value, mediaOpsTableColumnDetails?.isPartOfTierId ?? false, ComparisionOperator.Equal);
                    }
                    else
                    {
                        subResult += $"({table.Name}.{column.Name} = {value})";
                    }
                }
                result += $"({subResult})";
            }

            return result;
        }

        private async Task<Dictionary<string, List<string>>> GetDirectRunRestrictionsAsync(
            Guid omniClientId,
            ICollection<RunRestriction> restrictions,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            MediaopsHierarchyDetails activeTemplate,
            CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, List<string>>();

            // group by tableid and column to format the query with multiple conditions
            var restrictionGroup = (from r in restrictions
                                    group r by new { r.TableId, r.ColumnName } into groups
                                    select new
                                    {
                                        TableId = groups.Key,
                                        Items = groups.Select(c => c),
                                    }).ToList();

            foreach (var restrictionsGroup in restrictionGroup)
            {
                var items = restrictionsGroup.Items.ToList();
                if (!items.Any())
                    continue;


                var first = items.First();
                var (table, column, isAuxillary) = await GetTableColumnAsync(first.TableId, omniClientId, first.ColumnName, dictionaryTables, cancellationToken);
                var mediaOpsTableColumnDetails = GetColumnDetails(activeTemplate, table.Name, first.ColumnName);

                var formattedValues = new List<string>();
                foreach (var restriction in items)
                {
                    var formatted = column.Type switch
                    {
                        ColumnType.Boolean => SqlQuote(Json.Deserialize<bool>(restriction.ValueJson).ToString()),
                        ColumnType.Date => $"DATE({SqlQuote(Json.Deserialize<DateTime>(restriction.ValueJson).ToString("yyyy-MM-dd"))})",
                        ColumnType.Decimal => SqlQuote(Json.Deserialize<decimal>(restriction.ValueJson).ToString()!.Replace(",", ".")),
                        ColumnType.LongInteger => SqlQuote(Json.Deserialize<long>(restriction.ValueJson).ToString()),
                        ColumnType.String => SqlQuote(Json.Deserialize<string>(restriction.ValueJson)!),
                        ColumnType.Jsonb => SqlQuote(Json.Deserialize<string>(restriction.ValueJson)!),
                        _ => throw new NotSupportedException($"Column type {column.Type} is not supported."),
                    };
                    formattedValues.Add(formatted);
                }

                var distinctValues = string.Join(",", formattedValues.Distinct());

                if (mediaOpsTableColumnDetails.isAuxiliary)
                {
                    // Use auxiliary IN form
                    AddTodDictionary(result, table.Name, GetRunConstraintAuxiliary(table.Name, column.Name, distinctValues, mediaOpsTableColumnDetails?.isPartOfTierId ?? false, ComparisionOperator.In), mediaOpsTableColumnDetails);
                }
                else
                {
                    // Normal column IN (...)
                    AddTodDictionary(result, table.Name, $"{table.Name}.{column.Name} IN ({distinctValues})");
                }
            }

            return result;
        }

        private async Task<List<Column>> GetDisplayNameColumns(Guid tableId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var mediaOpsColumnSerachDTO = new MediaopsColumnSearchDTO()
            {
                ClientId = omniClientId.ToString()
            };
            var user = await userProvider.GetCurrentAsync(httpContextAccessor.HttpContext.User, cancellationToken) ?? throw new Exception("User not found.");
            var omniClientInfoDTO = await clientControllerLogic.GetAsync(omniClientId, user.Id, cancellationToken);
            if (omniClientInfoDTO != null)
            {
                mediaOpsColumnSerachDTO.Name = omniClientInfoDTO.Client.Name;
            }
            var ANsid = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var mediaOpsHierarchyDetails = await mediaopsControllerLogic.GetTableDataFromMediaopsAsync(mediaOpsColumnSerachDTO, ANsid, cancellationToken);
            var activeTemplate = mediaOpsHierarchyDetails.Where(c => c.isActive).FirstOrDefault();
            var table = await GetTableColumnsInfosAsync(tableId, omniClientId, cancellationToken);
            return activeTemplate?.columns?.Where(c => string.Equals(c.tier, table.Name, StringComparison.CurrentCultureIgnoreCase) && c.isPartOfDisplayName).ToList() ?? new List<Column>();
        }

        private async Task<(List<string> Selected, List<string> Where, List<string>? splitWhere)> GetSelectedColumnsAsync(
            Guid omniClientId,
            FlowchartDefinition flowchartDefinition,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            CancellationToken cancellationToken,
            Currency currency,
            MediaopsHierarchyDetails? template = null,
            bool isSplitWhere = false
            )
        {
            var selected = new List<string>();
            var where = new List<string>();
            bool isLocalCurrency = currency == Currency.LLL;
            string trimEndForLocal = isLocalCurrency ? "_local" : " ";
            var noneMetricTableGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var ANsid = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var activeTemplate = template ?? await GetMediaOpsHierarchyDetails(omniClientId, ANsid, cancellationToken);

            var minimumCount = 1;
            var whereSplitOrder = flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels.Count > 1 ? flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels
               .Where(x => x.Settings.Count(s => s.Enabled) > minimumCount) // Ensure levels with enabled settings are considered
                .MinBy(x => x.Order)?.Order ?? -1 : -1;

            var whereOptions = new List<string>();

            foreach (var level in flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels.OrderBy(x => x.Order))
            {
                level.Settings.ThrowIfNull().IfEmpty(x => x.Where(c => c.Enabled));

                var (table, column, isAuxillary) = await GetTableColumnAsync(level.TableId, omniClientId, level.ColumnName, dictionaryTables, cancellationToken);
                var mediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, level.ColumnName);
                var displayNameColumns = await GetDisplayNameColumns(table.Id, omniClientId, activeTemplate, cancellationToken);

                if (mediaOpsTableDetails != null)
                {
                    if (mediaOpsTableDetails.isAuxiliary)
                    {
                        var auxiliaryColumn = GetSelectAuxiliary(table.Name, column.Name, $"{MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", false, null, mediaOpsTableDetails.isPartOfTierId);
                        selected.Add(GetConcatenatedColumn(table.Name, column.Name, auxiliaryColumn, $"{MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", displayNameColumns));
                    }
                    else
                    {
                        selected.Add(GetConcatenatedColumn(table.Name, column.Name, $"{table.Name}.{column.Name} AS {MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", $"{MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", displayNameColumns));
                    }
                }
                else
                {
                    // Handle the case where mediaOpsTableDetails is null
                    log.Add(LogLevel.Error, "mediaOpsTableDetails is null for table: " + table.Name + ", column: " + column.Name);
                }
                var settings = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                var settingsMetric = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                var (settingsMetricTable, settingsMetricColumn) = (table, column);
                try
                {
#pragma warning disable CS8629 // Nullable value type may be null.
                    (settingsMetricTable, settingsMetricColumn, isAuxillary) = await GetTableColumnAsync(tableId: (Guid)level.TableId, omniClientId, level.ColumnName, dictionaryTables, cancellationToken);
#pragma warning restore CS8629 // Nullable value type may be null.
                    mediaOpsTableDetails = GetColumnDetails(activeTemplate, settingsMetricTable.Name, level.ColumnName);
                }
                catch (Exception ex)
                {
                    log.Add(LogLevel.Error, $"Error getting table and column for metric table id {level.TableId}", ex);
                }
                var values = string.Join(",", settingsMetric.Select(x => SqlQuote(x.Name.Split('~')[0])));

                if (mediaOpsTableDetails != null)
                {
                    if (isSplitWhere && whereSplitOrder == level.Order)
                    {
                        var rowCount = 1;
                        var maxLoop = Math.Ceiling(Convert.ToDouble(settingsMetric.Count() / rowCount));
                        Enumerable.Range(0, (int)maxLoop).Each(c =>
                        {
                            var splitValues = settingsMetric.OrderBy(x => x.Name).Skip(c * rowCount).Take(rowCount).Select(x => SqlQuote(x.Name.Split('~')[0]));
                            if (splitValues.Any())
                            {
                                var splitWhere = string.Join(",", splitValues);

                                if (mediaOpsTableDetails.isAuxiliary)
                                {
                                    whereOptions.Add(GetConstraintAuxiliary(table.Name, column.Name, splitWhere, mediaOpsTableDetails.isPartOfTierId, ComparisionOperator.In));
                                }
                                else
                                {
                                    whereOptions.Add($"{table.Name}.{column.Name} IN ({splitWhere})");
                                }
                            }
                        });
                    }
                    else
                    {
                        if (mediaOpsTableDetails.isAuxiliary)
                        {
                            where.Add(GetConstraintAuxiliary(settingsMetricTable.Name, column.Name, values, mediaOpsTableDetails.isPartOfTierId, ComparisionOperator.In));
                        }
                        else
                        {
                            where.Add($"{settingsMetricTable.Name}.{column.Name} IN ({values})");
                        }
                    }
                }
                else
                {
                    log.Add(LogLevel.Error, $"mediaOpsTableDetails is null for table: {settingsMetricTable.Name}, column: {column.Name}");
                }

                var uniqueSettings = settings.DistinctBy(c => new { c.MetricColumnName, c.MetricTableId });
                var uniqueSubTotals = settings.SelectMany(c => c.SubTotals ?? Enumerable.Empty<MediaHierarchySubTotal>())
                    .DistinctBy(c => new { c.ColumnName, c.TableId });
                var uniqueInflightOverlays = settings.SelectMany(c => c.InflightOverlays ?? Enumerable.Empty<MediaHierarchyInflightOverlay>())
                      .Where(overlay => overlay.ColumnName != "None" && overlay.ColumnName != CustomFlightRange && overlay.TableId != Guid.Empty)
                    .DistinctBy(c => new { c.ColumnName, c.TableId });


                foreach (var setting in uniqueSettings)
                {

                    if (setting.MetricColumnName != "None")
                    {
#pragma warning disable CS8629 // Nullable value type may be null.
                        (table, column, isAuxillary) = await GetTableColumnAsync((Guid)setting.MetricTableId, omniClientId, setting.MetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
#pragma warning restore CS8629 // Nullable value type may be null.
                        var metricMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, setting.MetricColumnName);

                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelMetricPrefix}_{level.Order}_{setting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, metricMediaOpsTableDetails?.isPartOfTierId ?? false));
                    }
                    else
                    {
                        (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                        var metricMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);

                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelMetricPrefix}_{level.Order}_{setting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, metricMediaOpsTableDetails?.isPartOfTierId ?? false));
                    }
                }


                foreach (var total in uniqueSubTotals)
                {

                    if (total.ColumnName != "None")
                    {
                        (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                        var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);

                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalPrefix}_{level.Order}_{total.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                    }
                    else
                    {
                        (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                        var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);

                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalPrefix}_{level.Order}_{total.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                    }
                }

                foreach (var inflightOverlay in uniqueInflightOverlays)
                {
                    var (retrievedTable, retrievedColumn, isAuxillaryColum) = await GetTableColumnAsync(inflightOverlay.TableId, omniClientId, inflightOverlay.ColumnName, dictionaryTables, cancellationToken);
                    var inflighOverlayMediaOpsTableDetails = GetColumnDetails(activeTemplate, retrievedTable.Name, inflightOverlay.ColumnName);
                    if (retrievedTable != null && retrievedColumn != null)
                    {
                        var tableName = retrievedTable.Name;
                        var columnName = retrievedColumn.Name;
                        var settingOrder = inflightOverlay.Order;

                        if (inflighOverlayMediaOpsTableDetails != null && inflighOverlayMediaOpsTableDetails.isAuxiliary)
                        {
                            selected.Add(GetSelectAuxiliary(tableName, columnName, $"{LevelInflightOverlayPrefix}_{level.Order}_{inflightOverlay.Order}_{inflightOverlay.Order}_{columnName}", false, null, inflighOverlayMediaOpsTableDetails?.isPartOfTierId ?? false));
                        }
                        else
                        {
                            selected.Add($"{tableName}.{columnName} AS {LevelInflightOverlayPrefix}_{level.Order}_{inflightOverlay.Order}_{inflightOverlay.Order}_{columnName}");
                        }
                    }
                }

                foreach (var setting in settings)
                {

                    if (setting.SubTotals != null)
                    {
                        setting.SubTotals.Throw().IfEmpty();
                    }

                    if (setting.SubLevels != null)
                    {
                        setting.SubLevels.Throw().IfEmpty();

                        foreach (var sublevel in setting.SubLevels)
                        {
                            sublevel.Settings.ThrowIfNull().IfEmpty(x => x.Where(c => c.Enabled));

                            (table, column, isAuxillary) = await GetTableColumnAsync(sublevel.TableId, omniClientId, sublevel.ColumnName, dictionaryTables, cancellationToken);
                            var subLevelMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, sublevel.ColumnName);
                            var subLeveldisplayNameColumns = await GetDisplayNameColumns(table.Id, omniClientId, cancellationToken);

                            if (subLevelMediaOpsTableDetails.isAuxiliary)
                            {
                                var auxiliaryColumn = GetSelectAuxiliary(table.Name, column.Name, $"{MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", false, null, subLevelMediaOpsTableDetails?.isPartOfTierId ?? false);
                                selected.Add(GetConcatenatedColumn(table.Name, column.Name, auxiliaryColumn, $"{MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", subLeveldisplayNameColumns));
                            }
                            else
                            {
                                selected.Add(GetConcatenatedColumn(table.Name, column.Name, $"{table.Name}.{column.Name} AS {MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", $"{MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", subLeveldisplayNameColumns));
                            }

                            var subsettings = sublevel.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                            foreach (var subsetting in subsettings)
                            {
                                if (subsetting.MetricColumnName != "None")
                                {
#pragma warning disable CS8629 // Nullable value type may be null.
                                    (table, column, isAuxillary) = await GetTableColumnAsync((Guid)subsetting.MetricTableId, omniClientId, subsetting.MetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
#pragma warning restore CS8629 // Nullable value type may be null.
                                    var subLevelSettingMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, subsetting.MetricColumnName);

                                    selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelMetricPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subLevelSettingMediaOpsTableDetails?.isPartOfTierId ?? false));
                                }
                                else
                                {
                                    (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    var subLevelSettingMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);

                                    selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelMetricPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subLevelSettingMediaOpsTableDetails?.isPartOfTierId ?? false));
                                }
                                if (subsetting.InflightOverlayTableId != null && subsetting.InflightOverlayTableId.ToString() != "00000000-0000-0000-0000-000000000001")
                                {
                                    subsetting.InflightOverlayColumnName.ThrowIfNull().IfEmpty();

                                    (table, column, isAuxillary) = await GetTableColumnAsync(subsetting.InflightOverlayTableId.Value, omniClientId, subsetting.InflightOverlayColumnName, dictionaryTables, cancellationToken);
                                    var subLevelSettingInflghtMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, subsetting.InflightOverlayColumnName);
                                    if (subLevelSettingInflghtMediaOpsTableDetails.isAuxiliary)
                                    {
                                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name}", false, null, subLevelSettingInflghtMediaOpsTableDetails?.isPartOfTierId ?? false));
                                    }
                                    else
                                    {
                                        selected.Add($"{table.Name}.{column.Name} AS {SubLevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name}");
                                    }
                                }

                                if (subsetting.SubTotals != null)
                                {
                                    subsetting.SubTotals.Throw().IfEmpty();

                                    foreach (var total in subsetting.SubTotals)
                                    {
                                        (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                        var subLevelSettingSubTotalMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);
                                        if (subLevelSettingSubTotalMediaOpsTableDetails.isAuxiliary)
                                        {
                                            selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelSubTotalPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subLevelSettingSubTotalMediaOpsTableDetails?.isPartOfTierId ?? false));
                                        }
                                        else
                                        {
                                            selected.Add($"{table.Name}.{column.Name} AS {SubLevelSubTotalPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (setting.SubTotalSummary != null)
                    {
                        setting.SubTotalSummary.Throw().IfEmpty();


                        foreach (var total in setting.SubTotalSummary.OrderBy(c => c.Order))
                        {
                            total.Settings.Throw().IfEmpty();

                            foreach (var totalSetting in total.Settings)
                            {
                                (table, column, isAuxillary) = await GetTableColumnAsync(totalSetting.TableId, omniClientId, totalSetting.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                displayNameColumns = await GetDisplayNameColumns(table.Id, omniClientId, activeTemplate, cancellationToken);
                                var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, totalSetting.ColumnName);
                                if (subTotalsMediaOpsTableDetails.isAuxiliary)
                                {
                                    selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{totalSetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", false, displayNameColumns, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                                }
                                else
                                {
                                    selected.Add(GetConcatenatedColumn(table.Name, column.Name, $"{table.Name}.{column.Name} AS {LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{totalSetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", $"{LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{totalSetting.Order}_{column.Name}", displayNameColumns));
                                }
                            }

                            total.SubTotals.Throw().IfEmpty();

                            foreach (var subTotal in total.SubTotals)
                            {
                                if (subTotal.ColumnName != "None")
                                {
                                    (table, column, isAuxillary) = await GetTableColumnAsync(subTotal.TableId, omniClientId, subTotal.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, subTotal.ColumnName);
                                    selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalSummaryMetricPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotal.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                                }
                                else
                                {
                                    (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);
                                    selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalSummaryMetricPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotal.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                                }
                            }
                        }
                    }
                }
            }

            if (flowchartDefinition.TotalsDefinition?.Definition != null)
            {
                foreach (var total in flowchartDefinition.TotalsDefinition.Definition.Columns)
                {
                    var (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                    var rightHandTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);

                    if (rightHandTotalsMediaOpsTableDetails != null)
                    {
                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{TotalsPrefix}{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, rightHandTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                    }
                }
            }

            if (flowchartDefinition.GrandTotalDefinition?.Definition != null)
            {
                flowchartDefinition.GrandTotalDefinition.Definition.Selections.ThrowIfNull().IfEmpty();

                foreach (var total in flowchartDefinition.GrandTotalDefinition.Definition.Selections.DistinctBy(c => new { c.ColumnName, c.TableId }))
                {
                    var (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                    var grandTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);

                    if (grandTotalsMediaOpsTableDetails != null)
                    {
                        selected.Add(GetSelectAuxiliary(table.Name, column.Name, $"{GrandTotalsPrefix}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, grandTotalsMediaOpsTableDetails?.isPartOfTierId ?? false));
                    }
                }
            }

            return (Selected: selected, Where: where, splitWhere: whereOptions);
        }

        private async Task<(Dictionary<string, List<string>> Selected, Dictionary<string, List<string>> Where)> GetDirectSelectedColumnsAsync(
                Guid omniClientId,
                FlowchartDefinition flowchartDefinition,
                List<DataDictionaryTableDetailsDTO> dictionaryTables,
                CancellationToken cancellationToken,
                Currency currency,
                MediaopsHierarchyDetails? template = null
        )
        {
            var selected = new Dictionary<string, List<string>>();
            var where = new Dictionary<string, List<string>>();
            bool isLocalCurrency = currency == Currency.LLL;
            string trimEndForLocal = isLocalCurrency ? "_local" : " ";
            var noneMetricTableGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var ANsid = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var activeTemplate = template ?? await GetMediaOpsHierarchyDetails(omniClientId, ANsid, cancellationToken);

            var minimumCount = 1;
            var whereSplitOrder = flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels.Count > 1 ? flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels
               .Where(x => x.Settings.Count(s => s.Enabled) > minimumCount) // Ensure levels with enabled settings are considered
                .MinBy(x => x.Order)?.Order ?? -1 : -1;

            var whereOptions = new List<string>();

            foreach (var level in flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels.OrderBy(x => x.Order))
            {
                level.Settings.ThrowIfNull().IfEmpty(x => x.Where(c => c.Enabled));

                var (table, column, isAuxillary) = await GetTableColumnAsync(level.TableId, omniClientId, level.ColumnName, dictionaryTables, cancellationToken);
                var mediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, level.ColumnName);
                var displayNameColumns = await GetDisplayNameColumns(table.Id, omniClientId, activeTemplate, cancellationToken);

                if (mediaOpsTableDetails != null)
                {
                    if (mediaOpsTableDetails.isAuxiliary)
                    {
                        var auxiliaryColumn = GetSelectAuxiliary(table.Name, column.Name, $"{MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", false, null, mediaOpsTableDetails.isPartOfTierId);

                        AddTodDictionary(selected, table.Name, GetConcatenatedColumn(table.Name, column.Name, auxiliaryColumn, $"{MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", displayNameColumns), mediaOpsTableDetails);
                    }
                    else
                    {
                        AddTodDictionary(selected, table.Name, GetConcatenatedColumn(table.Name, column.Name, $"{table.Name}.{column.Name} AS {MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", $"{MediaHierarchyLevelPrefix}{level.Order}_{column.Name}", displayNameColumns), mediaOpsTableDetails);
                    }
                }
                else
                {
                    // Handle the case where mediaOpsTableDetails is null
                    log.Add(LogLevel.Error, "mediaOpsTableDetails is null for table: " + table.Name + ", column: " + column.Name);
                }
                var settings = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                var settingsMetric = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                var (settingsMetricTable, settingsMetricColumn) = (table, column);
                try
                {
#pragma warning disable CS8629 // Nullable value type may be null.
                    (settingsMetricTable, settingsMetricColumn, isAuxillary) = await GetTableColumnAsync(tableId: (Guid)level.TableId, omniClientId, level.ColumnName, dictionaryTables, cancellationToken);
#pragma warning restore CS8629 // Nullable value type may be null.
                    mediaOpsTableDetails = GetColumnDetails(activeTemplate, settingsMetricTable.Name, level.ColumnName);
                }
                catch (Exception ex)
                {
                    log.Add(LogLevel.Error, $"Error getting table and column for metric table id {level.TableId}", ex);
                }
                var values = string.Join(",", settingsMetric.Select(x => SqlQuote(x.Name.Split('~')[0])));

                if (mediaOpsTableDetails != null)
                {
                    var constraint = mediaOpsTableDetails.isAuxiliary
                        ? GetConstraintAuxiliary(settingsMetricTable.Name, column.Name, values, mediaOpsTableDetails.isPartOfTierId, ComparisionOperator.In)
                        : $"{settingsMetricTable.Name}.{column.Name} IN ({values})";

                    AddTodDictionary(where, settingsMetricTable.Name, constraint, mediaOpsTableDetails);
                }
                else
                {
                    log.Add(LogLevel.Error, $"mediaOpsTableDetails is null for table: {settingsMetricTable.Name}, column: {column.Name}");
                }

                var uniqueSettings = settings.DistinctBy(c => new { c.MetricColumnName, c.MetricTableId });
                var uniqueSubTotals = settings.SelectMany(c => c.SubTotals ?? Enumerable.Empty<MediaHierarchySubTotal>())
                    .DistinctBy(c => new { c.ColumnName, c.TableId });
                var uniqueInflightOverlays = settings.SelectMany(c => c.InflightOverlays ?? Enumerable.Empty<MediaHierarchyInflightOverlay>())
                      .Where(overlay => overlay.ColumnName != "None" && overlay.ColumnName != CustomFlightRange && overlay.TableId != Guid.Empty)
                    .DistinctBy(c => new { c.ColumnName, c.TableId });


                foreach (var setting in uniqueSettings)
                {

                    if (setting.MetricColumnName != "None")
                    {
#pragma warning disable CS8629 // Nullable value type may be null.
                        (table, column, isAuxillary) = await GetTableColumnAsync((Guid)setting.MetricTableId, omniClientId, setting.MetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
#pragma warning restore CS8629 // Nullable value type may be null.
                        var metricMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, setting.MetricColumnName);

                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelMetricPrefix}_{level.Order}_{setting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, metricMediaOpsTableDetails?.isPartOfTierId ?? false), metricMediaOpsTableDetails);
                    }
                    else
                    {
                        (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                        var metricMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);

                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelMetricPrefix}_{level.Order}_{setting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, metricMediaOpsTableDetails?.isPartOfTierId ?? false), metricMediaOpsTableDetails);
                    }
                }


                foreach (var total in uniqueSubTotals)
                {

                    if (total.ColumnName != "None")
                    {
                        (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                        var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);

                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalPrefix}_{level.Order}_{total.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), subTotalsMediaOpsTableDetails);
                    }
                    else
                    {
                        (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                        var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);

                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalPrefix}_{level.Order}_{total.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), subTotalsMediaOpsTableDetails);
                    }
                }

                foreach (var inflightOverlay in uniqueInflightOverlays)
                {
                    var (retrievedTable, retrievedColumn, isAuxillaryColum) = await GetTableColumnAsync(inflightOverlay.TableId, omniClientId, inflightOverlay.ColumnName, dictionaryTables, cancellationToken);
                    var inflighOverlayMediaOpsTableDetails = GetColumnDetails(activeTemplate, retrievedTable.Name, inflightOverlay.ColumnName);
                    if (retrievedTable != null && retrievedColumn != null)
                    {
                        var tableName = retrievedTable.Name;
                        var columnName = retrievedColumn.Name;
                        var settingOrder = inflightOverlay.Order;

                        if (inflighOverlayMediaOpsTableDetails != null && inflighOverlayMediaOpsTableDetails.isAuxiliary)
                        {
                            AddTodDictionary(selected, tableName, GetSelectAuxiliary(tableName, columnName, $"{LevelInflightOverlayPrefix}_{level.Order}_{inflightOverlay.Order}_{inflightOverlay.Order}_{columnName}", false, null, inflighOverlayMediaOpsTableDetails?.isPartOfTierId ?? false), inflighOverlayMediaOpsTableDetails);
                        }
                        else
                        {
                            AddTodDictionary(selected, tableName, $"{tableName}.{columnName} AS {LevelInflightOverlayPrefix}_{level.Order}_{inflightOverlay.Order}_{inflightOverlay.Order}_{columnName}", inflighOverlayMediaOpsTableDetails);
                        }
                    }
                }

                foreach (var setting in settings)
                {

                    if (setting.SubTotals != null)
                    {
                        setting.SubTotals.Throw().IfEmpty();
                    }

                    if (setting.SubLevels != null)
                    {
                        setting.SubLevels.Throw().IfEmpty();

                        foreach (var sublevel in setting.SubLevels)
                        {
                            sublevel.Settings.ThrowIfNull().IfEmpty(x => x.Where(c => c.Enabled));

                            (table, column, isAuxillary) = await GetTableColumnAsync(sublevel.TableId, omniClientId, sublevel.ColumnName, dictionaryTables, cancellationToken);
                            var subLevelMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, sublevel.ColumnName);
                            var subLeveldisplayNameColumns = await GetDisplayNameColumns(table.Id, omniClientId, cancellationToken);

                            if (subLevelMediaOpsTableDetails.isAuxiliary)
                            {
                                var auxiliaryColumn = GetSelectAuxiliary(table.Name, column.Name, $"{MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", false, null, subLevelMediaOpsTableDetails?.isPartOfTierId ?? false);
                                AddTodDictionary(selected, table.Name, GetConcatenatedColumn(table.Name, column.Name, auxiliaryColumn, $"{MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", subLeveldisplayNameColumns), subLevelMediaOpsTableDetails);
                            }
                            else
                            {
                                AddTodDictionary(selected, table.Name, GetConcatenatedColumn(table.Name, column.Name, $"{table.Name}.{column.Name} AS {MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", $"{MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}", subLeveldisplayNameColumns), subLevelMediaOpsTableDetails);
                            }

                            var subsettings = sublevel.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                            foreach (var subsetting in subsettings)
                            {
                                if (subsetting.MetricColumnName != "None")
                                {
#pragma warning disable CS8629 // Nullable value type may be null.
                                    (table, column, isAuxillary) = await GetTableColumnAsync((Guid)subsetting.MetricTableId, omniClientId, subsetting.MetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
#pragma warning restore CS8629 // Nullable value type may be null.
                                    var subLevelSettingMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, subsetting.MetricColumnName);

                                    AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelMetricPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subLevelSettingMediaOpsTableDetails?.isPartOfTierId ?? false), subLevelSettingMediaOpsTableDetails);
                                }
                                else
                                {
                                    (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    var subLevelSettingMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);

                                    AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelMetricPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subLevelSettingMediaOpsTableDetails?.isPartOfTierId ?? false), subLevelSettingMediaOpsTableDetails);
                                }
                                if (subsetting.InflightOverlayTableId != null && subsetting.InflightOverlayTableId.ToString() != "00000000-0000-0000-0000-000000000001")
                                {
                                    subsetting.InflightOverlayColumnName.ThrowIfNull().IfEmpty();

                                    (table, column, isAuxillary) = await GetTableColumnAsync(subsetting.InflightOverlayTableId.Value, omniClientId, subsetting.InflightOverlayColumnName, dictionaryTables, cancellationToken);
                                    var subLevelSettingInflghtMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, subsetting.InflightOverlayColumnName);
                                    if (subLevelSettingInflghtMediaOpsTableDetails.isAuxiliary)
                                    {
                                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name}", false, null, subLevelSettingInflghtMediaOpsTableDetails?.isPartOfTierId ?? false), subLevelSettingInflghtMediaOpsTableDetails);
                                    }
                                    else
                                    {
                                        AddTodDictionary(selected, table.Name, $"{table.Name}.{column.Name} AS {SubLevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name}", subLevelSettingInflghtMediaOpsTableDetails);
                                    }
                                }

                                if (subsetting.SubTotals != null)
                                {
                                    subsetting.SubTotals.Throw().IfEmpty();

                                    foreach (var total in subsetting.SubTotals)
                                    {
                                        (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                        var subLevelSettingSubTotalMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);
                                        if (subLevelSettingSubTotalMediaOpsTableDetails.isAuxiliary)
                                        {
                                            AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{SubLevelSubTotalPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subLevelSettingSubTotalMediaOpsTableDetails?.isPartOfTierId ?? false), subLevelSettingSubTotalMediaOpsTableDetails);
                                        }
                                        else
                                        {
                                            AddTodDictionary(selected, table.Name, $"{table.Name}.{column.Name} AS {SubLevelSubTotalPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", subLevelSettingSubTotalMediaOpsTableDetails);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (setting.SubTotalSummary != null)
                    {
                        setting.SubTotalSummary.Throw().IfEmpty();


                        foreach (var total in setting.SubTotalSummary.OrderBy(c => c.Order))
                        {
                            total.Settings.Throw().IfEmpty();

                            foreach (var totalSetting in total.Settings)
                            {
                                (table, column, isAuxillary) = await GetTableColumnAsync(totalSetting.TableId, omniClientId, totalSetting.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                displayNameColumns = await GetDisplayNameColumns(table.Id, omniClientId, activeTemplate, cancellationToken);
                                var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, totalSetting.ColumnName);
                                if (subTotalsMediaOpsTableDetails.isAuxiliary)
                                {
                                    AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{totalSetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", false, displayNameColumns, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), subTotalsMediaOpsTableDetails);
                                }
                                else
                                {
                                    AddTodDictionary(selected, table.Name, GetConcatenatedColumn(table.Name, column.Name, $"{table.Name}.{column.Name} AS {LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{totalSetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", $"{LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{totalSetting.Order}_{column.Name}", displayNameColumns), subTotalsMediaOpsTableDetails);
                                }
                            }

                            total.SubTotals.Throw().IfEmpty();

                            foreach (var subTotal in total.SubTotals)
                            {
                                if (subTotal.ColumnName != "None")
                                {
                                    (table, column, isAuxillary) = await GetTableColumnAsync(subTotal.TableId, omniClientId, subTotal.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, subTotal.ColumnName);
                                    AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalSummaryMetricPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotal.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), subTotalsMediaOpsTableDetails);
                                }
                                else
                                {
                                    (table, column, isAuxillary) = await GetTableColumnAsync(DefaultMetricTableId, omniClientId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    var subTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, DefaultMetricColumnName);
                                    AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{LevelSubTotalSummaryMetricPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotal.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, subTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), subTotalsMediaOpsTableDetails);
                                }
                            }
                        }
                    }
                }
            }

            if (flowchartDefinition.TotalsDefinition?.Definition != null)
            {
                foreach (var total in flowchartDefinition.TotalsDefinition.Definition.Columns)
                {
                    var (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                    var rightHandTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);

                    if (rightHandTotalsMediaOpsTableDetails != null)
                    {
                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{TotalsPrefix}{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, rightHandTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), rightHandTotalsMediaOpsTableDetails);
                    }
                }
            }

            if (flowchartDefinition.GrandTotalDefinition?.Definition != null)
            {
                flowchartDefinition.GrandTotalDefinition.Definition.Selections.ThrowIfNull().IfEmpty();

                foreach (var total in flowchartDefinition.GrandTotalDefinition.Definition.Selections.DistinctBy(c => new { c.ColumnName, c.TableId }))
                {
                    var (table, column, isAuxillary) = await GetTableColumnAsync(total.TableId, omniClientId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                    var grandTotalsMediaOpsTableDetails = GetColumnDetails(activeTemplate, table.Name, total.ColumnName);

                    if (grandTotalsMediaOpsTableDetails != null)
                    {
                        AddTodDictionary(selected, table.Name, GetSelectAuxiliary(table.Name, column.Name, $"{GenericMetricPrefix}_{column.Name.Replace(trimEndForLocal, string.Empty)}", true, null, grandTotalsMediaOpsTableDetails?.isPartOfTierId ?? false), grandTotalsMediaOpsTableDetails);
                    }
                }
            }

            return (Selected: selected, Where: where);
        }

        // geneate a separete method to add the values to list in to dictionart<string, List<string>> and handle the key exists condition
        private void AddTodDictionary(Dictionary<string, List<string>> selected, string key, string value, Column? mediaOpsColumnDetails = null)
        {
            var updatedKey = key;
            if (mediaOpsColumnDetails is not null && mediaOpsColumnDetails.isAuxiliary && !mediaOpsColumnDetails.isPartOfTierId)
            {
                updatedKey = $"{key}_auxiliary";
            }

            if (selected.ContainsKey(updatedKey))
            {
                selected[updatedKey].Add(value);
            }
            else
            {
                selected[updatedKey] = new List<string> { value };
            }
        }

        private ICollection<FlowchartDataSubTotal>? GetSubTotals(
            ICollection<MediaHierarchySubTotal>? subTotals,
            string subTotalPrefix,
            int[] orders,
            List<Dictionary<string, object?>> data)
        {
            if (subTotals == null || !subTotals.Any())
            {
                return null;
            }

            var result = new Collection<FlowchartDataSubTotal>();

            foreach (var subtotal in subTotals.OrderBy(x => x.Order))
            {
                var order = new List<int>(orders)
                {
                    subtotal.Order,
                };

                result.Add(new FlowchartDataSubTotal
                {
                    ColumnName = subtotal.ColumnName,
                    TableId = subtotal.TableId,
                    Order = subtotal.Order,
                    Metrics = subtotal.ColumnName != "None" ? GetMetrics(subtotal.ColumnName, subTotalPrefix, order.ToArray(), null, string.Empty, data) : GetMetrics(DefaultMetricColumnName, subTotalPrefix, order.ToArray(), null, string.Empty, data),
                    FlightRange = subtotal.FlightRange,
                });
            }

            return result;
        }

        private ICollection<FlowchartDataSubTotal>? GetSubTotalSummary(
         ICollection<MediaHierarchySubTotalSummary>? subTotalSummary,
         string subTotalSummaryPrefix,
         int[] orders,
         List<Dictionary<string, object?>> bigdata)
        {
            if (subTotalSummary == null || !subTotalSummary.Any())
            {
                return null;
            }

            var result = new Collection<FlowchartDataSubTotal>();

            foreach (var subtotal in subTotalSummary.OrderBy(x => x.Order))
            {
                var order = new List<int>(orders)
                {
                    subtotal.Order,
                };

                var data = bigdata;
                var latestData = new Dictionary<string, List<Dictionary<string, object?>>>();
                for (var i = 0; i < subtotal.Settings.Count; i++)
                {
                    var setting = subtotal.Settings.ToList()[i];
                    var order1 = new List<int>(order)
                        {
                             setting.Order,
                        };
                    var additional = order1 == null ? string.Empty : $"_{string.Join("_", order1)}";
                    var settingField = $"{subTotalSummaryPrefix}{additional}_{setting.ColumnName}";
                    var values = setting.Values.Split(";");
                    var valuesJson = new Dictionary<string, string>();
                    if (i == 0)
                    {
                        var subData = data.Where(x => x.ContainsKey(settingField)).GroupBy(x => x[settingField]);
                        foreach (var value in values)
                        {

                            var settingData = subData.FirstOrDefault(x => (x.Key as string) == value);

                            if (settingData != null)
                            {

                                var list = settingData.ToList();
                                latestData.Add(value, list);
                            }
                        }
                    }
                    else
                    {
                        var newLatestData = new Dictionary<string, List<Dictionary<string, object?>>>();
                        foreach (var dict in latestData)
                        {
                            var subData = dict.Value.Where(x => x.ContainsKey(settingField)).GroupBy(x => x[settingField]);
                            foreach (var value in values)
                            {

                                var settingData = subData.FirstOrDefault(x => (x.Key as string) == value);

                                if (settingData != null)
                                {
                                    var list = settingData.ToList();
                                    newLatestData.Add($"{value} {dict.Key}", list);
                                }

                            }
                        }

                        latestData = newLatestData;
                    }
                }

                for (var metricsCount = 0; metricsCount < subtotal.SubTotals.Count; metricsCount++)
                {
                    var subTotal = subtotal.SubTotals.ToList()[metricsCount];
                    foreach (var dict in latestData)
                    {
                        var order1 = new List<int>(order)
                        {
                           subTotal.Order,
                        };
                        var additional = order1 == null ? string.Empty : $"_{string.Join("_", order1)}";
                        var metricField = $"{subTotalSummaryPrefix}{additional}_{subTotal.ColumnName}";
                        var metric = GetMetrics(subTotal.ColumnName, LevelSubTotalSummaryMetricPrefix, order1.ToArray(), null, string.Empty, dict.Value);

                        result.Add(new FlowchartDataSubTotal
                        {
                            ColumnName = subTotal.ColumnName,
                            TableId = subTotal.TableId,
                            Order = subtotal.Order,
                            Metrics = metric,
                            FlightRange = subTotal.FlightRange,
                            DisplayName = dict.Key,
                        });

                    }

                }
            }

            return result;
        }

        private async Task<DataDictionaryTableDetailsDTO> GetTableAsync(Guid id, Guid omniClientId, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken)
        {
            var table = dictionaryTables.Find(x => x.Id == id);

            if (table == null)
            {
                table = await GetTableColumnsInfosAsync(id, omniClientId, cancellationToken);
                dictionaryTables.Add(table);
            }

            return table;
        }

        private async Task<DataDictionaryTableDetailsDTO> GetTableByNameAsync(string name, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken)
        {
            var table = dictionaryTables.Find(x => x.Name == name);

            if (table == null)
            {
                table = mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByNameAsync(name, cancellationToken));
                dictionaryTables.Add(table);
            }

            return table;
        }
        private async Task<(DataDictionaryTableDetailsDTO Table, DataDictionaryColumnDetailsDTO Column, bool isAuxillary)>
                                    GetTableColumnAsync(Guid tableId, Guid omniClientId, string columnName, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken, bool isLocalCurrency = false)
        {
            bool isAuxillary = false;
            var table = await GetTableAsync(tableId, omniClientId, dictionaryTables, cancellationToken);

            var column = table.Columns?.FirstOrDefault(x => x.Name == columnName);
            if (column == null || column.Name == null)
            {
                column = new DataDictionaryColumnDetailsDTO
                {
                    Name = columnName,
                    Type = ColumnType.String,
                };
                isAuxillary = true;
            }

            return isLocalCurrency ? GetLocalMetricColumnAsync(table, column, isAuxillary) : (table, column, isAuxillary);
        }

        private ICollection<FlowchartDataTotal>? GetTotals(
            TotalsDefinition? totalsDefinition,
            List<Dictionary<string, object?>> data, bool isGrpMetric = false)
        {
            if (totalsDefinition == null)
            {
                return null;
            }
            var result = new Collection<FlowchartDataTotal>();

            var first = data.First();

            // Pre-compute serialization options once
            var jsonOptions = GetJsonSerializerOptions();

            // Use Parallel.ForEach for better performance with large collections
            var concurrentResults = new ConcurrentBag<FlowchartDataTotal>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, totalsDefinition.Columns.Count)
            };

            Parallel.ForEach(totalsDefinition.Columns, parallelOptions, total =>
            {
                var columnKey = $"{TotalsPrefix}{total.Order}_{total.ColumnName}";
                var genericKey = $"{GenericMetricPrefix}_{total.ColumnName}";

                var flowchartTotal = new FlowchartDataTotal
                {
                    ColumnName = total.ColumnName,
                    Name = total.Name,
                    Order = total.Order,
                    TableId = total.TableId,
                    ValuesJson = isGrpMetric && MetricCurrencyColumns.Contains(total.ColumnName)
                        ? new List<string>()
                        : data.AsParallel()
                            .Where(x => x.ContainsKey(genericKey) || x.ContainsKey(columnKey))
                            .Select(x =>
                            {
                                if (x.TryGetValue(genericKey, out var value) && value != null)
                                    return value;
                                if (x.TryGetValue(columnKey, out value) && value != null)
                                    return value;
                                return null;
                            })
                            .Where(x => x != null)
                            .Select(x => JsonSerializer.Serialize(x, jsonOptions))
                            .ToList()
                };

                concurrentResults.Add(flowchartTotal);
            });

            // Add results in the correct order
            foreach (var total in concurrentResults.OrderBy(x => x.Order))
            {
                result.Add(total);
            }

            return result;
        }

        // Below method is used to get the BriefedCtc when in Media Hierarchy Channel is selected
        private ICollection<FlowchartDataTotal>? GetBriefedCtcTotals(
            List<Dictionary<string, object?>> data)
        {
            var result = new Collection<FlowchartDataTotal>
            {
                new()
                {
                    ColumnName = AthenaConsts.BriefedCtc,
                    Name = AthenaConsts.Channel,
                    Order = 0, // not used
                    TableId = Guid.NewGuid(), // not used
                    ValuesJson = data.Where(x => x.ContainsKey($"{TotalsPrefix}_{AthenaConsts.BriefedCtc}")).Select(x => x[$"{TotalsPrefix}_{AthenaConsts.BriefedCtc}"]).Where(x => x != null).Select(x => Json.Serialize(x)).ToList(),
                },
            };

            return result;
        }

        private (DataDictionaryTableDetailsDTO Table, DataDictionaryColumnDetailsDTO Column, bool IsAuxillary)
                                    GetLocalMetricColumnAsync(DataDictionaryTableDetailsDTO table, DataDictionaryColumnDetailsDTO column, bool isAuxillary)
        {
            try
            {
                //isAuxillary = false;
                column = table.Columns?.FirstOrDefault(x => x.Name == $"{column.Name}_local") ?? throw new KeyNotFoundException($"Column {column.Name} not found in table {table.Name}");
                if (column == null || column.Name == null)
                {
                    column = new DataDictionaryColumnDetailsDTO
                    {
                        Name = $"{column.Name}_local",
                        Type = ColumnType.Decimal,
                    };
                    //isAuxillary = true;
                }
            }
            catch (Exception)
            {
                log.Add(LogLevel.Error, $"Error getting local currency column for {column.Name}");
            }
            return (table, column, isAuxillary);
        }
        private string GetConstraintAuxiliary(string tableName, string columnName, string value, bool isPartOfTierId = false, ComparisionOperator compare = ComparisionOperator.Equal)
        {
            StringBuilder query = new StringBuilder();
            if (isPartOfTierId)
            {
                query.Append($"{tableName}.{AthenaConsts.AuxiliaryPartOfTierId}->>'{columnName}'");
            }
            else
            {
                query.Append($"{tableName}_{AthenaConsts.Auxiliary}->>'{columnName}'");
            }
            if (compare == ComparisionOperator.In)
            {
                query.Append($" IN ({value})");
            }
            else
            {
                query.Append($"={SqlQuote(value)}");
            }
            return query.ToString();
        }

        private string GetRunConstraintAuxiliary(string tableName, string columnName, string value, bool isPartOfTierId = false, ComparisionOperator compare = ComparisionOperator.Equal)
        {
            StringBuilder query = new StringBuilder();
            if (isPartOfTierId)
            {
                query.Append($"{tableName}.{AthenaConsts.AuxiliaryPartOfTierId}->>'{columnName}'");
            }
            else
            {
                query.Append($"{tableName}_{AthenaConsts.Auxiliary}->>'{columnName}'");
            }
            if (compare == ComparisionOperator.In)
            {
                query.Append($" IN ({value})");
            }
            else
            {
                query.Append($"={value}");
            }
            return query.ToString();
        }

        private string GetAuxiliaryWhereClause(string tableName, string columnName)
        {
            return $"({tableName}_{AthenaConsts.Auxiliary}->>'{columnName}') ::timestamp";
        }

        //private string GetSelectAuxiliary(string tableName, string columnName, string alias, bool isMetric = false, List<Column> displayNames = null, bool isPartOfTierId = false)
        //{
        //    var select = new StringBuilder();

        //    if (!isMetric)
        //    {
        //        var differentColumns = displayNames?.Where(c => c.columnName != columnName).ToList();
        //        if (differentColumns?.Count > 0)
        //        {
        //            var baseSelect = isPartOfTierId ? $"{tableName}.{AthenaConsts.AuxiliaryPartOfTierId}->>'{columnName}'" : $"{tableName}_{AthenaConsts.Auxiliary}->>'{columnName}'";
        //            select.Append(GetConcatenatedColumn(tableName, columnName, baseSelect, alias, displayNames));
        //        }
        //        else
        //        {
        //            if (isPartOfTierId)
        //            {
        //                select.Append($"{tableName}.{AthenaConsts.AuxiliaryPartOfTierId}->>'{columnName}' AS {alias}");
        //            }
        //            else
        //            {
        //                select.Append($"{tableName}_{AthenaConsts.Auxiliary}->>'{columnName}' AS {alias}");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (isPartOfTierId)
        //        {
        //            select.Append($"({tableName}.{AthenaConsts.AuxiliaryPartOfTierId}->>'{columnName}') :: float AS {alias}");
        //        }
        //        else
        //        {
        //            select.Append($"({tableName}_{AthenaConsts.Auxiliary}->>'{columnName}') :: float AS {alias}");
        //        }
        //    }
        //    return select.ToString();
        //}

        private string GetSelectAuxiliary(string tableName, string columnName, string alias, bool isMetric = false, List<Column> displayNames = null, bool isPartOfTierId = false)
        {
            var select = new StringBuilder();
            var baseSelect = isPartOfTierId
                ? $"{tableName}.{AthenaConsts.AuxiliaryPartOfTierId}->>'{columnName}'"
                : $"{tableName}_{AthenaConsts.Auxiliary}->>'{columnName}'";

            if (!isMetric)
            {
                var differentColumns = displayNames?.Where(c => c.columnName != columnName).ToList();
                if (differentColumns?.Count > 0)
                {
                    select.Append(GetConcatenatedColumn(tableName, columnName, baseSelect, alias, displayNames));
                }
                else
                {
                    select.Append($"{baseSelect} AS {alias}");
                }
            }
            else
            {
                select.Append($"({baseSelect})::float AS {alias}");
            }

            return select.ToString();
        }

        private string GetConcatenatedColumn(string tableName, string columnName, string currentSelect, string alias, List<Column> displayNames)
        {
            var finalSelect = new StringBuilder();
            var differentColumns = displayNames?.Where(c => c.columnName != columnName).ToList();
            if (differentColumns?.Count > 0)
            {
                finalSelect.Append($"CONCAT({currentSelect.Split(" AS ")[0]},").Append("'~',");
                for (int i = 0; i < differentColumns.Count; i++)
                {
                    finalSelect.Append($"{tableName}.{differentColumns[i].columnName},");
                    if (i != differentColumns.Count - 1)
                    {
                        finalSelect.Append("'~',");
                    }
                }
                return finalSelect.ToString().TrimEnd(',') + ") AS " + alias;
            }
            else
            {
                return currentSelect;
            }
        }

        /// <summary>
        /// Get the Table Column Infos Async
        /// </summary>
        /// <param name="tableId">The Table Id</param>
        /// <param name="omniClientId">The Omni Client Id</param>
        /// <param name="cancellationToken"> The Cancellation Token</param>
        public async Task<DataDictionaryTableDetailsDTO> GetTableColumnsInfosAsync(Guid tableId, Guid omniClientId, CancellationToken cancellationToken)
        {

            // Generate a unique cache key based on tableId and omniClientId
            var cacheKey = $"TableColumns_{tableId}_{omniClientId}";

            return await cache.GetAsync(
                cacheKey,
                async ct =>
                {
                    var table = await portal.DataDictionaryTables.GetByIdAsync(tableId, cancellationToken);
                    var tableName = table.Name;
                    var clientId = portalDbContext.OmniClients.Where(x => x.Id == omniClientId).Select(x => x.ClientId).FirstOrDefault();
                    var clientVersion = clientId != null ? portalDbContext.ClientMapping.Where(x => x.ClientId == clientId).Select(x => x.Version).FirstOrDefault() : 1;
                    tableName = clientVersion > 1 ? $"{tableName}_v{clientVersion}" : tableName;
                    var columnInfos = await cache.GetAsync(
                        $"FlowchartData_Table_Schema_{tableId.GetHashCode()}",
                        x => queryLogic.GetTableColumnInfosAsync(tableName, x), cancellationToken);

                    table.DataDictionaryColumns = Json.Serialize(columnInfos.Select(x => mapper.Map<DataDictionaryColumn>(x)).ToList());
                    var result = mapper.Map<DataDictionaryTableDetailsDTO>(table);
                    result.Id = tableId;
                    result.Name = table.Name;
                    result.Removed = table.Removed;
                    return result;
                },
                cancellationToken);
        }

        private string GetClaimValue(string key)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(key)?.Value;
        }

        private Column GetColumnDetails(MediaopsHierarchyDetails activeTemplate, string tableName, string columnName)
        {
            var isBrandSelected = columnName.StartsWith("brand");
            var data = activeTemplate.columns.FirstOrDefault(c => c.tier == tableName && c.columnName.ToLower() == columnName.ToLower());
            if (isBrandSelected && (data == null || data.columnName == string.Empty))
            {
                Enumerable.Range(1, 10).ToList().ForEach(i =>
                {
                    var newColumnName = $"brand{i}";
                    data = activeTemplate.columns.FirstOrDefault(c => c.tier == tableName && c.columnName == newColumnName);
                    if (data != null && data.columnName != string.Empty)
                    {
                        return;
                    }
                });
            }
            return data;
        }

        private async Task<MediaopsHierarchyDetails> GetMediaOpsHierarchyDetails(Guid omniClientId, string ansid, CancellationToken cancellationToken)
        {
            string defaultClient = "default";
            var mediaOpsColumnSerachDTO = new MediaopsColumnSearchDTO()
            {
                ClientId = omniClientId.ToString()
            };
            var user = await userProvider.GetCurrentAsync(httpContextAccessor.HttpContext.User, cancellationToken) ?? throw new Exception("User not found.");
            var omniClientInfoDTO = await clientControllerLogic.GetAsync(omniClientId, user.Id, cancellationToken);
            if (omniClientInfoDTO != null)
            {
                mediaOpsColumnSerachDTO.Name = omniClientInfoDTO.Client.Name;
            }
            var mediaOpsHierarchyDetails = await mediaopsControllerLogic.GetTableDataFromMediaopsAsync(mediaOpsColumnSerachDTO, ansid, cancellationToken);

            var clientActiveTemplate = mediaOpsHierarchyDetails.Where(c => c.isActive && c.clientName == omniClientInfoDTO.Client.Name).Any();
            var isAnyDefaultTemplate = mediaOpsHierarchyDetails.Where(c => c.isActive && c.clientName == defaultClient).Any();
            var activeTemplate = mediaOpsHierarchyDetails.Where(c => c.isActive && (clientActiveTemplate ? c.clientName == omniClientInfoDTO.Client.Name : isAnyDefaultTemplate ? c.clientName == defaultClient : 1 == 1)).FirstOrDefault();
            return activeTemplate;
        }

        private async Task<List<Column>> GetDisplayNameColumns(Guid tableId, Guid omniClientId, MediaopsHierarchyDetails activeTemplate, CancellationToken cancellationToken)
        {
            var table = await GetTableColumnsInfosAsync(tableId, omniClientId, cancellationToken);
            return activeTemplate?.columns?.Where(c => string.Equals(c.tier, table.Name, StringComparison.CurrentCultureIgnoreCase) && c.isPartOfDisplayName).ToList() ?? new List<Column>();
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                PropertyNameCaseInsensitive = true,
            };
        }

    }

    enum ComparisionOperator
    {
        Equal,
        In
    }
}
