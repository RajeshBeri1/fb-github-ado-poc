using JasperFx.Core;
using Lib.Athena.Consts;
using Lib.Aurora.Business.Interfaces;
using Lib.Aurora.Models;
using Lib.MediaopsToFlowChart.DTO;
using Lib.MediaopsToFlowChart.Models;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using System.Diagnostics;
using Throw;
using Column = Lib.MediaopsToFlowChart.Models.Column;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// MediaopsControllerLogic
    /// </summary>
    public class MediaopsControllerLogic
    {
        private readonly IPortalDbContext portalDbContext;
        private readonly IAuroraQueryLogic queryLogic;
        private readonly ICacheLogic cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaopsControllerLogic"/> class.
        /// The MediaopsControllerLogic constructor
        /// </summary>
        /// <param name="portalDbContext">The portalDbContext.</param>
        /// <param name="queryLogic">The queryLogic.</param>
        /// <param name="cache">The cache.</param>
        public MediaopsControllerLogic(IPortalDbContext portalDbContext, IAuroraQueryLogic queryLogic, ICacheLogic cache)
        {
            this.portalDbContext = portalDbContext;
            this.queryLogic = queryLogic;
            this.cache = cache;
        }

        /// <summary>
        /// Gets the user profile asynchronous.
        /// </summary>
        /// <param name="mediaopsColumnSearchDTO">The mediaopsColumnSearchDTO.</param>
        /// <param name="omniClientInfoDTO">The omniClientInfoDTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<FlowchartDataDictionaryColumnDetails>> GetTableAndColumnDetailsFromMediaopsAsync(MediaopsColumnSearchDTO mediaopsColumnSearchDTO, OmniClientInfoDTO omniClientInfoDTO, CancellationToken cancellationToken)
        {
            if (omniClientInfoDTO != null)
            {
                mediaopsColumnSearchDTO.Name = omniClientInfoDTO.Client.Name;
            }
            var templatesClientColumnAliasesList = await cache.GetAsync(
                $"Flowchart_ClientColumnAliase_MediOps_{mediaopsColumnSearchDTO.ClientId.GetDeterministicHashCode()}",
                x => queryLogic.GetClientColumnDetailsByClientIdAndOmniGuid(
                    mediaopsColumnSearchDTO.Name,
                    Guid.Parse(mediaopsColumnSearchDTO?.ClientId),
                    x),
                cancellationToken);

            var mediaopsHierarchyDetails = await GetMediaopsHierarchyDetails(templatesClientColumnAliasesList);

            var flowchartDataDictionaryColumnDetails = new List<FlowchartDataDictionaryColumnDetails>();
            if (mediaopsHierarchyDetails.Count > 0)
            {
                string defaultClient = "default";

                var clientName = omniClientInfoDTO.Client.Name;
                var clientActiveTemplate = mediaopsHierarchyDetails.Any(c => c.isActive && c.clientName == clientName);
                var isAnyDefaultTemplate = mediaopsHierarchyDetails.Any(c => c.isActive && c.clientName == defaultClient);
                var activeTemplate = mediaopsHierarchyDetails
                    .Where(c => c.isActive && (clientActiveTemplate ? c.clientName == clientName : isAnyDefaultTemplate ? c.clientName == defaultClient : true))
                    .OrderByDescending(x => x.updatedAt)
                    .Take(1)
                    .ToList();

                // Use cached table and field info
                var fieldInfos = await GetCachedFieldInfosAsync(cancellationToken);
                var tables = await GetCachedDataDictionaryTablesAsync(cancellationToken);

                // Build lookup dictionaries for fast access
                var fieldInfoDict = fieldInfos
                    .GroupBy(f => f.Name.ToLowerInvariant())
                    .ToDictionary(g => g.Key, g => g.First());

                var tableDict = tables
                    .GroupBy(t => t.Name.ToLowerInvariant())
                    .ToDictionary(g => g.Key, g => g.First().Id);

                var addedColumns = new HashSet<(string TableName, string ColumnName)>();

                foreach (var item in activeTemplate)
                {
                    item.columns.ThrowIfNull();

                    var tableNames = item.columns
                        .Where(y => y.tier != AthenaConsts.Client && !string.IsNullOrWhiteSpace(y.tier))
                        .Select(x => x.tier?.ToLowerInvariant() ?? string.Empty)
                        .Distinct()
                        .ToList();

                    foreach (var table in tableNames)
                    {
                        if (!tableDict.TryGetValue(table, out var tableId) || tableId == Guid.Empty)
                            continue;

                        foreach (var columns in item.columns.Where(x => x.tier?.ToLowerInvariant() == table))
                        {
                            var columnNameUpper = columns.columnName?.ToLowerInvariant();
                            if (columnNameUpper == null || !fieldInfoDict.TryGetValue(columnNameUpper, out var columnDetails))
                                continue;

                            var columnKey = (table, columnNameUpper);
                            if (!addedColumns.Add(columnKey))
                                continue;

                            flowchartDataDictionaryColumnDetails.Add(new FlowchartDataDictionaryColumnDetails
                            {
                                TableId = tableId,
                                TableName = table,
                                ColumnName = columnDetails.Name,
                                DisplayName = columns.columnalias,
                                ColumnType = columnDetails.Type,
                                IsCommon = columnDetails.IsCommon,
                                IsCurrency = columnDetails.IsCurrency,
                                IsMetric = columnDetails.IsMetric,
                            });
                        }
                    }
                }
            }
            return flowchartDataDictionaryColumnDetails;
        }

        private async Task<List<FieldInfo>> GetCachedFieldInfosAsync(CancellationToken cancellationToken)
        {
            return await cache.GetAsync(
                "FieldInfos_All",
                async ct => await portalDbContext.FieldInfos.AsNoTracking().ToListAsync(ct),
                cancellationToken);
        }

        private async Task<List<DataDictionaryTable>> GetCachedDataDictionaryTablesAsync(CancellationToken cancellationToken)
        {
            return await cache.GetAsync(
                "DataDictionaryTables_All",
                async ct => await portalDbContext.DataDictionaryTables.AsNoTracking().ToListAsync(ct),
                cancellationToken);
        }

        /// <summary>
        /// Gets the user profile asynchronous.
        /// </summary>
        /// <param name="mediaopsColumnSearchDTO">The mediaopsColumnSearchDTO.</param>
        /// <param name="ansid">The ansid </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<MediaopsHierarchyDetails>> GetTableDataFromMediaopsAsync(MediaopsColumnSearchDTO mediaopsColumnSearchDTO, string ansid, CancellationToken cancellationToken)
        {
            var templatesClientColumnAliasesList = await cache.GetAsync($"Flowchart_ClientColumnAliase_MediOps_{mediaopsColumnSearchDTO.ClientId.GetDeterministicHashCode()}", x => queryLogic.GetClientColumnDetailsByClientIdAndOmniGuid(mediaopsColumnSearchDTO.Name, Guid.Parse(mediaopsColumnSearchDTO?.ClientId), x), cancellationToken);

            var clientColumnDetail = await GetMediaopsHierarchyDetails(templatesClientColumnAliasesList);

            return clientColumnDetail ?? new List<MediaopsHierarchyDetails>();
        }

        public async Task<List<MediaopsHierarchyDetails>> GetMediaopsHierarchyDetails(List<TemplatesClientColumnAliases> items)
        {
            var first = items.FirstOrDefault();
            var mediaopsDetail = new MediaopsHierarchyDetails
            {
                createdAt = first?.last_refreshed_at,
                createdBy = null,
                updatedAt = first?.last_refreshed_at,
                updatedBy = null,
                id = first?.template_tracking_id,
                omniguid = first?.omniguid,
                columns = new List<Column>(),
                minimumSetGuid = first?.template_uid,
                auxiliaryGuid = first?.template_uid,
                planIds = null,
                templateName = first?.client_name != null ? first.client_name + "_default" : null,
                clientName = first?.client_name,
                isActive = true,
                mediaOpsState = "Submitted",
                campaignStart = null,
                campaignEnd = null
            };

            foreach (var item in items)
            {
                mediaopsDetail.columns.Add(new Column
                {
                    createdAt = item.last_refreshed_at,
                    createdBy = null,
                    updatedAt = item.last_refreshed_at,
                    updatedBy = null,
                    templateGuid = item.template_uid,
                    tier = item.tier,
                    tierOrder = item.tier switch
                    {
                        var t when string.Equals(t, AthenaConsts.ClientTable, StringComparison.OrdinalIgnoreCase) => 1,
                        var t when string.Equals(t, AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase) => 2,
                        var t when string.Equals(t, AthenaConsts.Channel, StringComparison.OrdinalIgnoreCase) => 3,
                        var t when string.Equals(t, AthenaConsts.Budget, StringComparison.OrdinalIgnoreCase) => 4,
                        var t when string.Equals(t, AthenaConsts.Supplier, StringComparison.OrdinalIgnoreCase) => 5,
                        var t when string.Equals(t, AthenaConsts.Placement, StringComparison.OrdinalIgnoreCase) => 6,
                        _ => 0
                    },
                    columnName = item.pmds_column_name,
                    columnalias = item.columnalias,
                    sourceColumnName = item.pmds_column_name,
                    dataType = item.data_type,
                    sourceSystem = item.source_sys,
                    isPartOfTierId = item.is_part_of_tier_id,
                    isAGroupByColumn = item.is_a_group_by_column,
                    isMinSetColumn = item.designation == AthenaConsts.MinimumSet,
                    isPartOfDisplayName = false, //item.is_part_of_display_name,
                    isAuxiliary = item.designation == AthenaConsts.Auxiliary,
                });
            }

            return new List<MediaopsHierarchyDetails> { mediaopsDetail };
        }
    }
}
