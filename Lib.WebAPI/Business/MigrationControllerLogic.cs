using AutoMapper;
using JasperFx.Core;
using Lib.Athena.Consts;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business.Interfaces;
using Lib.MediaopsToFlowChart.Models;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models.Flowchart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// MigrationControllerLogic
    /// </summary>
    public class MigrationControllerLogic : IMigrationControllerLogic
    {
        private static readonly Guid MediaPlansTableId = Guid.Parse("F0867476-5D91-4FBF-8EBF-C5308309F4EA");
        private static readonly Guid MediaBriefsTableId = Guid.Parse("3E185D41-9E21-44DC-9532-CD4BDA1B5A0D");
        private readonly IPortalUnitOfWork portal;
        private readonly ILog<IMigrationControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalDbContext portalDbContext;
        private readonly IClientControllerLogic clientControllerLogic;
        private readonly IHubContext<NotificationHub, INotificationClient> hubContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserProvider userProvider;
        private readonly IAuroraQueryLogic queryLogic;
        private readonly ICacheLogic cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationControllerLogic"/> class.
        /// The MigrationControllerLogic constructor
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        /// <param name="clientControllerLogic">The clientControllerLogic</param>
        /// <param name="httpContextAccessor">The httpContextAccessor</param>
        /// <param name="userProvider">The userProvider</param>
        public MigrationControllerLogic(IPortalUnitOfWork portal, ILog<IMigrationControllerLogic> log, IMapper mapper, IPortalDbContext portalDbContext, IClientControllerLogic clientControllerLogic, IHubContext<NotificationHub, INotificationClient> hubContext, IHttpContextAccessor httpContextAccessor, IUserProvider userProvider, IAuroraQueryLogic queryLogic, ICacheLogic cache)
        {
            this.portal = portal;
            this.log = log;
            this.mapper = mapper;
            this.portalDbContext = portalDbContext;
            this.clientControllerLogic = clientControllerLogic;
            this.hubContext = hubContext;
            this.httpContextAccessor = httpContextAccessor;
            this.userProvider = userProvider;
            this.queryLogic = queryLogic;
            this.cache = cache;
        }

        /// <summary>
        /// Create the migration data asynchronous.
        /// </summary>
        /// <param name="createPlannedMediaMigrationDTO">The createPlannedMediaMigrationDTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CreateMigrationAsync(List<CreatePlannedMediaMigrationDTO> createPlannedMediaMigrationDTO, CancellationToken cancellationToken)
        {
            // Mapping the Planned Media To Test Schema Mapping to Planned Media To Test Schema Mapping

            var mappingDetails = await portalDbContext.PlannedMediaMigrationColumnMappings.ToListAsync();
            createPlannedMediaMigrationDTO.ThrowIfNull(nameof(createPlannedMediaMigrationDTO));

            var dataInsert = mappingDetails.Count > 0 ? createPlannedMediaMigrationDTO.Where(x => !mappingDetails.Any(y => y.SourceColumnName == x.SourceColumnName && y.SourceTableId == x.SourceTableId)).ToList() : createPlannedMediaMigrationDTO;

            if (dataInsert.Count > 0)
            {
                var plannedMediaToTestSchemaMappingInfo = mapper.Map<List<PlannedMediaMigrationColumnMapping>>(dataInsert);
                await portal.PlannedMediaMigrationColumnMappings.AddRangeAsync(plannedMediaToTestSchemaMappingInfo, cancellationToken);
                await portal.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the migration data asynchronous.
        /// </summary>
        /// <param name="updatePlannedMediaMigrationDTO">The updatePlannedMediaMigrationDTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateMigrationAsync(UpdatePlannedMediaMigrationDTO updatePlannedMediaMigrationDTO, CancellationToken cancellationToken)
        {
            updatePlannedMediaMigrationDTO.ThrowIfNull(nameof(updatePlannedMediaMigrationDTO));
            var plannedMediaToTestSchemaMapping = await portalDbContext.PlannedMediaMigrationColumnMappings.FirstOrDefaultAsync(x => x.Id == updatePlannedMediaMigrationDTO.Id, cancellationToken: cancellationToken);
            plannedMediaToTestSchemaMapping.ThrowIfNull(nameof(plannedMediaToTestSchemaMapping));
            plannedMediaToTestSchemaMapping.SourceColumnName = updatePlannedMediaMigrationDTO.SourceColumnName;
            plannedMediaToTestSchemaMapping.SourceTableId = updatePlannedMediaMigrationDTO.SourceTableId;
            plannedMediaToTestSchemaMapping.DestinationColumnName = updatePlannedMediaMigrationDTO.DestinationColumnName;
            plannedMediaToTestSchemaMapping.DestinationTableId = updatePlannedMediaMigrationDTO.DestinationTableId;
            plannedMediaToTestSchemaMapping.Removed = updatePlannedMediaMigrationDTO.Removed;
            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the migration list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        public async Task<List<PlannedMediaMigrationColumnMapping>> GetAllAsync(CancellationToken cancellationToken, bool removed = false)
        {
            var plannedMediaToTestSchemaMappingList = await portal.PlannedMediaMigrationColumnMappings.GetAllAsync(cancellationToken, removed);
            plannedMediaToTestSchemaMappingList.ThrowIfNull(nameof(plannedMediaToTestSchemaMappingList));
            return (List<PlannedMediaMigrationColumnMapping>)plannedMediaToTestSchemaMappingList;
        }


        /// <summary>
        /// Deletes the migration asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var fieldInfos = await portal.PlannedMediaMigrationColumnMappings.GetByIdAsync(id, cancellationToken);

            log.Add(LogLevel.Information, $"Deleting Planned Media To Test Schema Mapping record: {fieldInfos.Id}");

            await portal.PlannedMediaMigrationColumnMappings.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Migrate flowchart defintion by using omniGuid asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omniClientId identifier.</param>
        /// <param name="connectionId">The connectionId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> MigrateClientToNewVersionAsync(Guid omniClientId, string connectionId, CancellationToken cancellationToken)
        {
            bool isMigrationSuccess = false;
            string clientName = string.Empty;

            try
            {
                var mappingDetails = await portalDbContext.PlannedMediaMigrationColumnMappings.Where(x => x.Removed == false).OrderByDescending(x => x.OmniClientId).ToListAsync();
                List<string>? duplicates = null;
                var omniClientDetails = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(omniClientId, cancellationToken);
                clientName = omniClientDetails.Client.Name;
                Guid clientId = Guid.Parse(omniClientDetails.Client.ClientId);
                var clientMappingDetails = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
                clientMappingDetails.ThrowIfNull(nameof(clientMappingDetails));

                if (clientMappingDetails.Version < 2)
                {
                    await MigrateClientByOmniGuid(omniClientId, clientId, clientMappingDetails, mappingDetails, duplicates, cancellationToken);
                    clientMappingDetails.Version = 2;
                    await portal.SaveChangesAsync(cancellationToken);
                    log.Add(LogLevel.Information, $"Schema migration completed successfully for Client : {omniClientDetails.Client.Name}.");
                    isMigrationSuccess = true;
                    await hubContext.Clients.All.ReceiveNotification($"Migration completed for client {clientName} to version 2", false, connectionId);
                }
                else
                {
                    isMigrationSuccess = false;
                    log.Add(LogLevel.Information, "Schema migration failed because client is already in newer version.");
                    await hubContext.Clients.All.ReceiveNotification($"Migration failed for client {clientName} to version 2 as it already in newer version", true, connectionId);
                }
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Critical, ex.Message, ex);
                log.Add(LogLevel.Information, "Schema migration unsuccessfully.");
                isMigrationSuccess = false;
                await hubContext.Clients.All.ReceiveNotification($"Migration failed for client {clientName} to version 2", true, connectionId);
            }
            return isMigrationSuccess;
        }

        /// <summary>
        /// Migrate flowchart defintion to V1 by using omniGuid asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omniClientId identifier.</param>
        /// <param name="connectionId">The connectionId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> MigrateClientToInitialVersionAsync(Guid omniClientId, string connectionId, CancellationToken cancellationToken)
        {
            bool isMigrationSuccess = false;
            string clientName = string.Empty;
            try
            {
                var mappingDetails = await portalDbContext.PlannedMediaMigrationColumnMappings.Where(x => x.Removed == false).OrderByDescending(x => x.OmniClientId).ToListAsync();
                var duplicates = mappingDetails.GroupBy(i => i.SourceColumnName).Where(x => x.Count() > 1).Select(val => val.Key).ToList();
                var omniClientDetails = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(omniClientId, cancellationToken);
                clientName = omniClientDetails.Client.Name;
                Guid clientId = Guid.Parse(omniClientDetails.Client.ClientId);
                var clientMappingDetails = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
                clientMappingDetails.ThrowIfNull(nameof(clientMappingDetails));

                if (clientMappingDetails.Version > 1)
                {
                    await MigrateClientByOmniGuid(omniClientId, clientId, clientMappingDetails, mappingDetails, duplicates, cancellationToken);
                    clientMappingDetails.Version = 1;
                    await portal.SaveChangesAsync(cancellationToken);
                    log.Add(LogLevel.Information, $"Schema migration completed successfully for Client : {omniClientDetails.Client.Name}.");
                    isMigrationSuccess = true;
                    await hubContext.Clients.All.ReceiveNotification($"Migration completed for client {clientName} to version 1", false, connectionId);
                }
                else
                {
                    isMigrationSuccess = false;
                    log.Add(LogLevel.Information, "Schema migration failed because client is already in newer version.");
                    await hubContext.Clients.All.ReceiveNotification($"Migration failed for client {clientName} to version 1 as it already in intial version", true, connectionId);
                }
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Critical, ex.Message, ex);
                log.Add(LogLevel.Information, "Schema migration unsuccessfully.");
                isMigrationSuccess = false;
                await hubContext.Clients.All.ReceiveNotification($"Migration failed for client {clientName} to version 1", true, connectionId);
            }
            return isMigrationSuccess;
        }

        /// <summary>
        /// Get MediaHierarchy Column Used Details asynchronous.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="omniClientId">The omniClientId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<MediaHierarchyColumnsDetailsResultDTO> GetMediaHierarchyColumnUsesDetailsAsync(string? name, Guid? omniClientId, CancellationToken cancellationToken)
        {
            var omniClientInfoDTO = await (from c in portalDbContext.Clients
                                           join oc in portalDbContext.OmniClients on c.Id equals oc.ClientId
                                           select new OmniClientInfoDTO
                                           {
                                               Client = new ClientInfoDTO
                                               {
                                                   Name = c.Name,
                                                   ClientId = c.ClientId.ToString(),
                                               },
                                               Id = oc.Id,
                                           }).AsNoTracking().ToListAsync(cancellationToken);

            var mediaHierarchyDetails = (omniClientId.HasValue && omniClientId != Guid.Empty) ? await portalDbContext.MediaHierarchyTemplates.AsNoTracking().Where(z => z.OmniClientId == omniClientId && z.CreatedByUserId != Guid.Parse("2EF8B6C9-9B64-490D-A57B-E163D9496DC7") && !z.IsDefault).Select(x => new { x.MediaHierarchyDefinition, x.OmniClientId }).ToListAsync(cancellationToken) : await portalDbContext.MediaHierarchyTemplates.AsNoTracking().Where(z => z.CreatedByUserId != Guid.Parse("2EF8B6C9-9B64-490D-A57B-E163D9496DC7") && !z.IsDefault).Select(x => new { x.MediaHierarchyDefinition, x.OmniClientId }).ToListAsync(cancellationToken);
            List<MediaHierarchyDefinitionWithOmniClientId> mediaHierarchyDefinitionWithOmniClientIdList = new List<MediaHierarchyDefinitionWithOmniClientId>();
            foreach (var mediaHierarchy in mediaHierarchyDetails)
            {
                var mediaHierarchyDefinitionWithOmniClientId = new MediaHierarchyDefinitionWithOmniClientId
                {
                    OmniClientId = mediaHierarchy.OmniClientId,
                    ClientId = omniClientInfoDTO.FirstOrDefault(x => x.Id == mediaHierarchy.OmniClientId)?.Client?.ClientId ?? string.Empty,
                    ClientName = omniClientInfoDTO.FirstOrDefault(x => x.Id == mediaHierarchy.OmniClientId)?.Client?.Name ?? string.Empty,
                    Definition = JsonSerializer.Deserialize<MediaHierarchyDefinition>(mediaHierarchy.MediaHierarchyDefinition, GetJsonSerializerOptions()),
                };
                mediaHierarchyDefinitionWithOmniClientIdList.Add(mediaHierarchyDefinitionWithOmniClientId);
            }

            var omniClientIdWithClientIdList = mediaHierarchyDefinitionWithOmniClientIdList.Where(x => x.OmniClientId.HasValue).Select(x => new OmniClientIdWithClientId() { OmniClientId = x.OmniClientId!.Value, ClientId = x.ClientId, ClientName = x.ClientName }).Distinct().ToList();

            string allData = "allData";
            name = string.IsNullOrEmpty(name) ? allData : name;
            var clientColumnAliasesList = await cache.GetAsync($"Flowchart_ClientColumnAliaseInfo_{name.GetHashCode()}", x => queryLogic.GetClientColumnInfoByClientIdAndOmniGuid(name, omniClientId, x), cancellationToken);

            var clientColumnDetail = await GetMediaHierarchyColumnsDetailss(clientColumnAliasesList, name, omniClientId, omniClientIdWithClientIdList);
            return await GetMediaHierarchyColumnsUsedDetails(mediaHierarchyDefinitionWithOmniClientIdList, omniClientIdWithClientIdList, clientColumnDetail, cancellationToken);
        }

        public async Task<List<MediaHierarchyColumnsDetailss>> GetMediaHierarchyColumnsDetailss(List<ClientColumnAliases> items, string? clientId,
            Guid? omniGuid,
            List<OmniClientIdWithClientId>? omniClientIdWithClientIdList)
        {
            if (omniClientIdWithClientIdList == null || omniClientIdWithClientIdList.Count == 0)
                return new List<MediaHierarchyColumnsDetailss>();

            var clientDetailsList = new List<MediaHierarchyColumnsDetailss>(omniClientIdWithClientIdList.Count);

            foreach (var omniClient in omniClientIdWithClientIdList)
            {
                if (omniClient?.OmniClientId == null || string.IsNullOrWhiteSpace(omniClient.ClientId))
                    continue;

                var omniGuidStr = omniClient.OmniClientId.Value.ToString();
                var clientIdStr = omniClient.ClientId;

                // Use StringComparison.OrdinalIgnoreCase for case-insensitive comparison
                var rows = items.Where(x =>
                    !string.IsNullOrEmpty(x.omniguid) &&
                    !string.IsNullOrEmpty(x.client_id) &&
                    string.Equals(x.omniguid, omniGuidStr, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.client_id, clientIdStr, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                if (rows.Count == 0)
                    continue;

                var columnTableInfoList = new List<MediaHierarchyColumnAndTableDetailss>(rows.Count);
                foreach (var i in rows)
                {
                    var tableId = i.tier switch
                    {
                        var t when string.Equals(t, AthenaConsts.ClientTable, StringComparison.OrdinalIgnoreCase) => Guid.Parse("069a1838-6b78-49c1-9a19-8aa7156d8acd"),
                        var t when string.Equals(t, AthenaConsts.CampaignTable, StringComparison.OrdinalIgnoreCase) => Guid.Parse("6F2CE863-ECE8-442C-B5FB-BD59075914A9"),
                        var t when string.Equals(t, AthenaConsts.Channel, StringComparison.OrdinalIgnoreCase) => Guid.Parse("E13DDA1A-48A3-4E26-8A76-620BB10A2C6C"),
                        var t when string.Equals(t, AthenaConsts.Budget, StringComparison.OrdinalIgnoreCase) => Guid.Parse("E5CDF0AB-0455-43F7-953E-6B230513C3E6"),
                        var t when string.Equals(t, AthenaConsts.Supplier, StringComparison.OrdinalIgnoreCase) => Guid.Parse("0D771867-CC9A-40A5-900E-F57528F91D0E"),
                        var t when string.Equals(t, AthenaConsts.Placement, StringComparison.OrdinalIgnoreCase) => Guid.Parse("A1B4A058-D33D-4E9B-950C-5CA3CD438A7C"),
                        _ => Guid.Parse("797eaf39-e05e-43b4-b0e6-95130ce54925"),
                    };
                    columnTableInfoList.Add(new MediaHierarchyColumnAndTableDetailss
                    {
                        ColumnName = i.pmds_column_name,
                        TableId = tableId
                    });
                }

                var firstRow = rows[0];
                clientDetailsList.Add(new MediaHierarchyColumnsDetailss
                {
                    OmniClientId = Guid.TryParse(firstRow.omniguid, out var guidVal) ? guidVal : Guid.Empty,
                    ClientName = firstRow.client_name,
                    ClientId = firstRow.client_id,
                    ColumnInfoss = columnTableInfoList
                        .GroupBy(c => new { c.ColumnName, c.TableId })
                        .Select(g => g.First())
                        .ToList()
                });
            }

            // Remove duplicates and order by ClientName
            return clientDetailsList
                .GroupBy(x => new { x.OmniClientId, x.ClientId, x.ClientName })
                .Select(g => g.First())
                .OrderBy(x => x.ClientName)
                .ToList();
        }

        /// <summary>
        /// Gets the tableId asynchronous.
        /// </summary>
        /// <param name="sourceColumName">The columName.</param>
        /// <param name="sourceTableId">The sourceTableId.</param></param>
        /// <param name="duplicates">The duplicates.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private static Task<Guid> GetTableId(string sourceColumName, Guid sourceTableId, List<string> duplicates, List<string>? metrics = null)
        {
            Guid tableId = Guid.Empty;
            if (sourceColumName == AthenaConsts.BriefedCtc || (metrics != null && metrics.Any(x => x == AthenaConsts.BriefedCtc)))
            {
                tableId = MediaBriefsTableId;
            }
            else if (duplicates.Any(x => x == sourceColumName && x != AthenaConsts.BriefedCtc))
            {
                tableId = MediaPlansTableId;
            }
            tableId = tableId == Guid.Empty ? sourceTableId : tableId;

            return Task.FromResult(tableId);
        }

        /// <summary>
        /// Gets the updated flowcharts details for PMDS.
        /// </summary>
        /// <param name="flowcharts">The flowchart definition details DTO.</param>
        /// <param name="ft">The Flowchart template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<FlowchartTemplate> GetUpdatedFlowchartForPMDS(FlowchartDefinitionDetailsDTO flowcharts, FlowchartTemplate ft, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            flowcharts.Definition = await UpdateFlowchartDefinition(flowcharts.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            ft = mapper.Map(flowcharts, ft);
            return ft;
        }

        /// <summary>
        /// Gets the updated header details for PMDS.
        /// </summary>
        /// <param name="res">The header definition details DTO.</param>
        /// <param name="ht">The Header template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<HeaderTemplate> GetUpdatedHeaderForPMDS(HeaderDefinitionDetailsDTO res, HeaderTemplate ht, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            res.Definition = await UpdateHeaderDefinition(res.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            ht = mapper.Map(res, ht);
            return ht;
        }

        /// <summary>
        /// Gets the updated MediaHierarchy details for PMDS.
        /// </summary>
        /// <param name="res">The MediaHierarchy definition details DTO.</param>
        /// <param name="mht">The Media Hierarchy template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<MediaHierarchyTemplate> GetUpdatedMediaHierarchyForPMDS(MediaHierarchyDefinitionDetailsDTO res, MediaHierarchyTemplate mht, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            res.Definition = await UpdateMediaHierarchyDefinition(res.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            mht = mapper.Map(res, mht);
            return mht;
        }

        /// <summary>
        /// Gets the updated right hand totals details for PMDS.
        /// </summary>
        /// <param name="res">The right hand totals definition details DTO.</param>
        /// <param name="rht">The Right hand Total template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<TotalsTemplate> GetUpdatedRightHandTotalsForPMDS(RightHandTotalsDefinitionDetailsDTO res, TotalsTemplate rht, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            res.Definition = await UpdateRightHandTotalsDefinition(res.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            rht = mapper.Map(res, rht);
            return rht;
        }

        /// <summary>
        /// Gets the updated grand total details for PMDS.
        /// </summary>
        /// <param name="res">The grand total definition details DTO.</param>
        /// <param name="gtt">The Grand Total template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<GrandTotalTemplate> GetUpdatedGrandTotalForPMDS(GrandTotalDefintionDetailsDTO res, GrandTotalTemplate gtt, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            res.Definition = await UpdateGrandTotalDefinition(res.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            gtt = mapper.Map(res, gtt);
            return gtt;
        }

        /// <summary>
        /// Gets the updated themes details for PMDS.
        /// </summary>
        /// <param name="themes">The theme definition details DTO.</param>
        /// <param name="tt">The Theme template</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<ThemeTemplate> GetUpdatedThemeTemplateForPMDS(ThemeDefinitionDetailsDTO themes, ThemeTemplate tt, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            themes.Definition = await UpdateThemeDefinition(themes.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            tt = mapper.Map(themes, tt);
            return tt;
        }

        /// <summary>
        /// Gets the updated flowchartVersionHistorties details for PMDS.
        /// </summary>
        /// <param name="flowchartVersionHistorties">The flowchartVersionHistorties definition details DTO.</param>
        /// <param name="ftv">The flowchart template version history</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<FlowchartTemplatesVersionHistorty> GetUpdatedFlowchartTemplatesVersionHistortiesForPMDS(FlowchartTemplatesVersionHistortiesDefinitionDetailsDTO flowchartVersionHistorties, FlowchartTemplatesVersionHistorty ftv, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            flowchartVersionHistorties.Definition = await UpdateFlowchartDefinition(flowchartVersionHistorties.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            ftv = mapper.Map(flowchartVersionHistorties, ftv);
            return ftv;
        }

        /// <summary>
        /// Gets the updated RunConfiguration details for PMDS.
        /// </summary>
        /// <param name="res">The RunConfiguration definition details DTO.</param>
        /// <param name="rc">The Run configuration</param>
        /// <param name="clientversion">The client version.</param>
        /// <param name="mappingDetails">The mapping details.</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <param name="omniClientId">The omniClientId</param>
        public async Task<RunConfiguration> GetUpdatedRunConfigurationForPMDS(RunConfigurationDefinitionDetailsDTO res, RunConfiguration rc, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken, Guid? omniClientId = null)
        {
            bool switchToNewerVersion = clientversion < 2 ? true : false;
            res.MediaHierarchyLevels = await UpdateMediaHierarchyLevelDefinitionDefinition(res.MediaHierarchyLevels, switchToNewerVersion, mappingDetails, duplicates);
            res.RunRestrictions = await UpdateRunRestrictionsDefinition(res.RunRestrictions, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
            rc = mapper.Map(res, rc);
            return rc;
        }

        /// <summary>
        /// Updates the flowchart definition.
        /// </summary>
        /// <param name="definition">The Flowchart definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<FlowchartDefinition> UpdateFlowchartDefinition(FlowchartDefinition? definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (definition != null)
            {
                if (definition.HeaderDefinition != null && definition.HeaderDefinition.Definition != null)
                {
                    definition.HeaderDefinition.Definition = await UpdateHeaderDefinition(definition.HeaderDefinition.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
                }

                if (definition.TotalsDefinition != null && definition.TotalsDefinition.Definition != null)
                {
                    definition.TotalsDefinition.Definition = await UpdateRightHandTotalsDefinition(definition.TotalsDefinition.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
                }

                if (definition.GrandTotalDefinition != null && definition.GrandTotalDefinition.Definition != null)
                {
                    definition.GrandTotalDefinition.Definition = await UpdateGrandTotalDefinition(definition.GrandTotalDefinition.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
                }

                if (definition.MediaHierarchyDefinition != null && definition.MediaHierarchyDefinition.Definition.Levels != null)
                {
                    definition.MediaHierarchyDefinition.Definition = await UpdateMediaHierarchyDefinition(definition.MediaHierarchyDefinition.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
                }

                if (definition.ThemeDefinition != null && definition.ThemeDefinition?.Definition?.LegendTheme != null)
                {
                    definition.ThemeDefinition.Definition = await UpdateThemeDefinition(definition.ThemeDefinition.Definition, switchToNewerVersion, mappingDetails, duplicates, omniClientId);
                }
            }
            return definition;
        }

        /// <summary>
        /// Updates the header definition.
        /// </summary>
        /// <param name="definition">The Header definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<HeaderDefinition> UpdateHeaderDefinition(HeaderDefinition definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (definition.Configuration != null && definition.Configuration.Rows != null)
            {
                foreach (var headerRows in definition.Configuration.Rows)
                {
                    var headerRowDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == headerRows.TableId && x.SourceColumnName == headerRows.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == headerRows.TableId && x.DestinationColumnName == headerRows.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                    if (headerRowDetails != null)
                    {
                        if (switchToNewerVersion)
                        {
                            headerRows.ColumnName = headerRowDetails.DestinationColumnName;
                            headerRows.TableId = headerRowDetails.DestinationTableId;
                        }
                        else
                        {
                            headerRows.ColumnName = headerRowDetails.SourceColumnName;
                            headerRows.TableId = await GetTableId(headerRowDetails.SourceColumnName, headerRowDetails.SourceTableId, duplicates);
                        }
                    }
                }
            }

            if (definition.Configuration != null && definition.Configuration.Details != null)
            {
                foreach (var headerDetailLevelRows in definition.Configuration.Details.Rows)
                {
                    var headerDetailRows = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == headerDetailLevelRows.TableId && x.SourceColumnName == headerDetailLevelRows.ColumnName) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == headerDetailLevelRows.TableId && x.DestinationColumnName == headerDetailLevelRows.ColumnName);
                    if (headerDetailRows != null)
                    {
                        if (switchToNewerVersion)
                        {
                            headerDetailLevelRows.ColumnName = headerDetailRows.DestinationColumnName;
                            headerDetailLevelRows.TableId = headerDetailRows.DestinationTableId;
                        }
                        else
                        {
                            headerDetailLevelRows.ColumnName = headerDetailRows.SourceColumnName;
                            headerDetailLevelRows.TableId = await GetTableId(headerDetailRows.SourceColumnName, headerDetailRows.SourceTableId, duplicates);
                        }
                    }
                }
            }
            return definition;
        }

        /// <summary>
        /// Updates the media hierarchy definition.
        /// </summary>
        /// <param name="definition">The MediaHierarchy definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<MediaHierarchyDefinition> UpdateMediaHierarchyDefinition(MediaHierarchyDefinition definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (definition != null)
            {
                foreach (var lvl in definition.Levels)
                {
                    var finalLevelDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == lvl.TableId && x.SourceColumnName == lvl.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == lvl.TableId && x.DestinationColumnName == lvl.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                    if (finalLevelDetails != null)
                    {
                        if (switchToNewerVersion)
                        {
                            lvl.ColumnName = finalLevelDetails.DestinationColumnName;
                            lvl.TableId = finalLevelDetails.DestinationTableId;
                        }
                        else
                        {
                            lvl.ColumnName = finalLevelDetails.SourceColumnName;
                            lvl.TableId = await GetTableId(finalLevelDetails.SourceColumnName, finalLevelDetails.SourceTableId, duplicates, lvl.Settings.Select(x => x.MetricColumnName).ToList());
                        }
                    }

                    foreach (var set in lvl.Settings)
                    {
                        var finalSettingsInflightOverlayDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == set.InflightOverlayTableId && x.SourceColumnName == set.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == set.InflightOverlayTableId && x.DestinationColumnName == set.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                        if (finalSettingsInflightOverlayDetails != null)
                        {
                            if (switchToNewerVersion)
                            {
                                set.InflightOverlayColumnName = finalSettingsInflightOverlayDetails?.DestinationColumnName;
                                set.InflightOverlayTableId = finalSettingsInflightOverlayDetails?.DestinationTableId;
                            }
                            else
                            {
                                set.InflightOverlayColumnName = finalSettingsInflightOverlayDetails?.SourceColumnName;
                                set.InflightOverlayTableId = await GetTableId(finalSettingsInflightOverlayDetails.SourceColumnName, finalSettingsInflightOverlayDetails.SourceTableId, duplicates);
                            }
                        }

                        var finalSettingsMetricDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == set.MetricTableId && x.SourceColumnName == set.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == set.MetricTableId && x.DestinationColumnName == set.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                        if (finalSettingsMetricDetails != null)
                        {
                            if (switchToNewerVersion)
                            {
                                set.MetricColumnName = finalSettingsMetricDetails.DestinationColumnName;
                                set.MetricTableId = finalSettingsMetricDetails.DestinationTableId;
                            }
                            else
                            {
                                set.MetricColumnName = finalSettingsMetricDetails.SourceColumnName;
                                set.MetricTableId = await GetTableId(finalSettingsMetricDetails.SourceColumnName, finalSettingsMetricDetails.SourceTableId, duplicates);
                            }
                        }

                        if (set.SubLevels != null)
                        {
                            foreach (var subLevelDetails in set.SubLevels)
                            {
                                var finalSubLevelsDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelDetails.TableId && x.SourceColumnName == subLevelDetails.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelDetails.TableId && x.DestinationColumnName == subLevelDetails.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                if (finalSubLevelsDetails != null)
                                {
                                    if (switchToNewerVersion)
                                    {
                                        subLevelDetails.ColumnName = finalSubLevelsDetails.DestinationColumnName;
                                        subLevelDetails.TableId = finalSubLevelsDetails.DestinationTableId;
                                    }
                                    else
                                    {
                                        subLevelDetails.ColumnName = finalSubLevelsDetails.SourceColumnName;
                                        subLevelDetails.TableId = await GetTableId(finalSubLevelsDetails.SourceColumnName, finalSubLevelsDetails.SourceTableId, duplicates, subLevelDetails.Settings.Select(x => x.MetricColumnName).ToList());
                                    }
                                }

                                foreach (var subSet in subLevelDetails.Settings)
                                {
                                    var finalSubSettingsInflightOverlayDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subSet.InflightOverlayTableId && x.SourceColumnName == subSet.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subSet.InflightOverlayTableId && x.DestinationColumnName == subSet.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                    if (finalSubSettingsInflightOverlayDetails != null)
                                    {
                                        if (switchToNewerVersion)
                                        {
                                            subSet.InflightOverlayColumnName = finalSubSettingsInflightOverlayDetails.DestinationColumnName;
                                            subSet.InflightOverlayTableId = finalSubSettingsInflightOverlayDetails.DestinationTableId;
                                        }
                                        else
                                        {
                                            subSet.InflightOverlayColumnName = finalSubSettingsInflightOverlayDetails.SourceColumnName;
                                            subSet.InflightOverlayTableId = await GetTableId(finalSubSettingsInflightOverlayDetails.SourceColumnName, finalSubSettingsInflightOverlayDetails.SourceTableId, duplicates);
                                        }
                                    }

                                    var finalSubSettingsMetricDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subSet.MetricTableId && x.SourceColumnName == subSet.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subSet.MetricTableId && x.DestinationColumnName == subSet.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                    if (finalSubSettingsMetricDetails != null)
                                    {
                                        if (switchToNewerVersion)
                                        {
                                            subSet.MetricColumnName = finalSubSettingsMetricDetails.DestinationColumnName;
                                            subSet.MetricTableId = finalSubSettingsMetricDetails.DestinationTableId;
                                        }
                                        else
                                        {
                                            subSet.MetricColumnName = finalSubSettingsMetricDetails.SourceColumnName;
                                            subSet.MetricTableId = await GetTableId(finalSubSettingsMetricDetails.SourceColumnName, finalSubSettingsMetricDetails.SourceTableId, duplicates);
                                        }
                                    }

                                    if (subSet.SubTotals != null)
                                    {
                                        foreach (var subLevelSubTotal in subSet.SubTotals)
                                        {
                                            var finalSubLevelSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelSubTotal.TableId && x.SourceColumnName == subLevelSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelSubTotal.TableId && x.DestinationColumnName == subLevelSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                            if (finalSubLevelSubTotalDetails != null)
                                            {
                                                if (switchToNewerVersion)
                                                {
                                                    subLevelSubTotal.ColumnName = finalSubLevelSubTotalDetails.DestinationColumnName;
                                                    subLevelSubTotal.TableId = finalSubLevelSubTotalDetails.DestinationTableId;
                                                }
                                                else
                                                {
                                                    subLevelSubTotal.ColumnName = finalSubLevelSubTotalDetails.SourceColumnName;
                                                    subLevelSubTotal.TableId = await GetTableId(finalSubLevelSubTotalDetails.SourceColumnName, finalSubLevelSubTotalDetails.SourceTableId, duplicates);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (subLevelDetails.SubTotals != null)
                                {
                                    foreach (var subTotalLevels in subLevelDetails.SubTotals)
                                    {
                                        var finalSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subTotalLevels.TableId && x.SourceColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subTotalLevels.TableId && x.DestinationColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                        if (finalSubTotalDetails != null)
                                        {
                                            if (switchToNewerVersion)
                                            {
                                                subTotalLevels.ColumnName = finalSubTotalDetails.DestinationColumnName;
                                                subTotalLevels.TableId = finalSubTotalDetails.DestinationTableId;
                                            }
                                            else
                                            {
                                                subTotalLevels.ColumnName = finalSubTotalDetails.SourceColumnName;
                                                subTotalLevels.TableId = await GetTableId(finalSubTotalDetails.SourceColumnName, finalSubTotalDetails.SourceTableId, duplicates);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (set.SubTotals != null)
                        {
                            foreach (var subLevelsSubTotal in set.SubTotals)
                            {
                                var subLevelsSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelsSubTotal.TableId && x.SourceColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelsSubTotal.TableId && x.DestinationColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                if (subLevelsSubTotalDetails != null)
                                {
                                    if (switchToNewerVersion)
                                    {
                                        subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.DestinationColumnName;
                                        subLevelsSubTotal.TableId = subLevelsSubTotalDetails.DestinationTableId;
                                    }
                                    else
                                    {
                                        subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.SourceColumnName;
                                        subLevelsSubTotal.TableId = await GetTableId(subLevelsSubTotalDetails.SourceColumnName, subLevelsSubTotalDetails.SourceTableId, duplicates);
                                    }
                                }
                            }
                        }

                        if (set.SubTotalSummary != null)
                        {
                            foreach (var subTotalSummary in set.SubTotalSummary)
                            {
                                foreach (var subLevelsSubTotal in subTotalSummary.SubTotals)
                                {
                                    var subLevelsSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelsSubTotal.TableId && x.SourceColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelsSubTotal.TableId && x.DestinationColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                    if (subLevelsSubTotalDetails != null)
                                    {
                                        if (switchToNewerVersion)
                                        {
                                            subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.DestinationColumnName;
                                            subLevelsSubTotal.TableId = subLevelsSubTotalDetails.DestinationTableId;
                                        }
                                        else
                                        {
                                            subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.SourceColumnName;
                                            subLevelsSubTotal.TableId = await GetTableId(subLevelsSubTotalDetails.SourceColumnName, subLevelsSubTotalDetails.SourceTableId, duplicates);
                                        }
                                    }
                                }

                                foreach (var subLevelsSubTotal in subTotalSummary.Settings)
                                {
                                    var subLevelsSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelsSubTotal.TableId && x.SourceColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelsSubTotal.TableId && x.DestinationColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                    if (subLevelsSubTotalDetails != null)
                                    {
                                        if (switchToNewerVersion)
                                        {
                                            subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.DestinationColumnName;
                                            subLevelsSubTotal.TableId = subLevelsSubTotalDetails.DestinationTableId;
                                        }
                                        else
                                        {
                                            subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.SourceColumnName;
                                            subLevelsSubTotal.TableId = await GetTableId(subLevelsSubTotalDetails.SourceColumnName, subLevelsSubTotalDetails.SourceTableId, duplicates);
                                        }
                                    }
                                }
                            }
                        }

                        if (set.InflightOverlays != null)
                        {
                            foreach (var inflightOverlays in set.InflightOverlays)
                            {
                                var inflightOverlaysDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == inflightOverlays.TableId && x.SourceColumnName == inflightOverlays.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == inflightOverlays.TableId && x.DestinationColumnName == inflightOverlays.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                if (inflightOverlaysDetails != null)
                                {
                                    if (switchToNewerVersion)
                                    {
                                        inflightOverlays.ColumnName = inflightOverlaysDetails.DestinationColumnName;
                                        inflightOverlays.TableId = inflightOverlaysDetails.DestinationTableId;
                                    }
                                    else
                                    {
                                        inflightOverlays.ColumnName = inflightOverlaysDetails.SourceColumnName;
                                        inflightOverlays.TableId = await GetTableId(inflightOverlaysDetails.SourceColumnName, inflightOverlaysDetails.SourceTableId, duplicates);
                                    }
                                }
                            }
                        }
                    }

                    if (lvl.SubTotals != null)
                    {
                        foreach (var subTotalLevels in lvl.SubTotals)
                        {
                            var finalSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subTotalLevels.TableId && x.SourceColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subTotalLevels.TableId && x.DestinationColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                            if (finalSubTotalDetails != null)
                            {
                                if (switchToNewerVersion)
                                {
                                    subTotalLevels.ColumnName = finalSubTotalDetails.DestinationColumnName;
                                    subTotalLevels.TableId = finalSubTotalDetails.DestinationTableId;
                                }
                                else
                                {
                                    subTotalLevels.ColumnName = finalSubTotalDetails.SourceColumnName;
                                    subTotalLevels.TableId = await GetTableId(finalSubTotalDetails.SourceColumnName, finalSubTotalDetails.SourceTableId, duplicates);
                                }
                            }
                        }
                    }
                }
            }
            return definition;
        }

        /// <summary>
        /// Updates the right hand totals definition.
        /// </summary>
        /// <param name="definition">The RightHandTotals definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<TotalsDefinition> UpdateRightHandTotalsDefinition(TotalsDefinition definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (definition.Columns != null)
            {
                foreach (var rhtLevel in definition.Columns)
                {
                    var rhtLevelDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == rhtLevel.TableId && x.SourceColumnName == rhtLevel.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == rhtLevel.TableId && x.DestinationColumnName == rhtLevel.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                    if (rhtLevelDetails != null)
                    {
                        if (switchToNewerVersion)
                        {
                            rhtLevel.ColumnName = rhtLevelDetails.DestinationColumnName;
                            rhtLevel.TableId = rhtLevelDetails.DestinationTableId;
                        }
                        else
                        {
                            rhtLevel.ColumnName = rhtLevelDetails.SourceColumnName;
                            rhtLevel.TableId = await GetTableId(rhtLevelDetails.SourceColumnName, rhtLevelDetails.SourceTableId, duplicates);
                        }
                    }
                }
            }
            return definition;
        }

        /// <summary>
        /// Updates the grand total definition.
        /// </summary>
        /// <param name="definition">The GrandTotals definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<GrandTotalDefinition> UpdateGrandTotalDefinition(GrandTotalDefinition definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            foreach (var gttLevel in definition.Selections)
            {
                var gttLevelDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == gttLevel.TableId && x.SourceColumnName == gttLevel.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == gttLevel.TableId && x.DestinationColumnName == gttLevel.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                if (gttLevelDetails != null)
                {
                    if (switchToNewerVersion)
                    {
                        gttLevel.ColumnName = gttLevelDetails.DestinationColumnName;
                        gttLevel.TableId = gttLevelDetails.DestinationTableId;
                    }
                    else
                    {
                        gttLevel.ColumnName = gttLevelDetails.SourceColumnName;
                        gttLevel.TableId = await GetTableId(gttLevelDetails.SourceColumnName, gttLevelDetails.SourceTableId, duplicates);
                    }

                }
            }
            return definition;
        }

        /// <summary>
        /// Updates the theme definition.
        /// </summary>
        /// <param name="definition">The Theme definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<ThemeDefinition> UpdateThemeDefinition(ThemeDefinition definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (definition.LegendTheme != null)
            {
                var finalThemeDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == definition.LegendTheme.TableId && x.SourceColumnName == definition.LegendTheme.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == definition.LegendTheme.TableId && x.DestinationColumnName == definition.LegendTheme.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                if (finalThemeDetails != null)
                {
                    if (switchToNewerVersion)
                    {
                        definition.LegendTheme.ColumnName = finalThemeDetails.DestinationColumnName;
                        definition.LegendTheme.TableId = finalThemeDetails.DestinationTableId;
                    }
                    else
                    {
                        definition.LegendTheme.ColumnName = finalThemeDetails.SourceColumnName;
                        definition.LegendTheme.TableId = await GetTableId(finalThemeDetails.SourceColumnName, finalThemeDetails.SourceTableId, duplicates);
                    }
                }
            }
            return definition;
        }

        /// <summary>
        /// Updates the media hierarchy level definition.
        /// </summary>
        /// <param name="mediaHierarchyLevelDefinition">The MediaHeirarchyLevel definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<ICollection<MediaHierarchyLevelBase>> UpdateMediaHierarchyLevelDefinitionDefinition(ICollection<MediaHierarchyLevelBase>? mediaHierarchyLevelDefinition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (mediaHierarchyLevelDefinition != null)
            {
                foreach (var lvl in mediaHierarchyLevelDefinition)
                {
                    var finalLevelDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == lvl.TableId && x.SourceColumnName == lvl.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == lvl.TableId && x.DestinationColumnName == lvl.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                    if (finalLevelDetails != null)
                    {
                        if (switchToNewerVersion)
                        {
                            lvl.ColumnName = finalLevelDetails.DestinationColumnName;
                            lvl.TableId = finalLevelDetails.DestinationTableId;
                        }
                        else
                        {
                            lvl.ColumnName = finalLevelDetails.SourceColumnName;
                            lvl.TableId = await GetTableId(finalLevelDetails.SourceColumnName, finalLevelDetails.SourceTableId, duplicates, lvl.Settings.Select(x => x.MetricColumnName).ToList());
                        }
                    }

                    foreach (var set in lvl.Settings)
                    {
                        var finalSettingsInflightOverlayDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == set.InflightOverlayTableId && x.SourceColumnName == set.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == set.InflightOverlayTableId && x.DestinationColumnName == set.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                        if (finalSettingsInflightOverlayDetails != null)
                        {
                            if (switchToNewerVersion)
                            {
                                set.InflightOverlayColumnName = finalSettingsInflightOverlayDetails?.DestinationColumnName;
                                set.InflightOverlayTableId = finalSettingsInflightOverlayDetails?.DestinationTableId;
                            }
                            else
                            {
                                set.InflightOverlayColumnName = finalSettingsInflightOverlayDetails?.SourceColumnName;
                                set.InflightOverlayTableId = await GetTableId(finalSettingsInflightOverlayDetails.SourceColumnName, finalSettingsInflightOverlayDetails.SourceTableId, duplicates);
                            }
                        }

                        var finalSettingsMetricDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == set.MetricTableId && x.SourceColumnName == set.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == set.MetricTableId && x.DestinationColumnName == set.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                        if (finalSettingsMetricDetails != null)
                        {
                            if (switchToNewerVersion)
                            {
                                set.MetricColumnName = finalSettingsMetricDetails.DestinationColumnName;
                                set.MetricTableId = finalSettingsMetricDetails.DestinationTableId;
                            }
                            else
                            {
                                set.MetricColumnName = finalSettingsMetricDetails.SourceColumnName;
                                set.MetricTableId = await GetTableId(finalSettingsMetricDetails.SourceColumnName, finalSettingsMetricDetails.SourceTableId, duplicates);
                            }
                        }

                        if (set.SubLevels != null)
                        {
                            foreach (var subLevelDetails in set.SubLevels)
                            {
                                var finalSubLevelsDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelDetails.TableId && x.SourceColumnName == subLevelDetails.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelDetails.TableId && x.DestinationColumnName == subLevelDetails.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                if (finalSubLevelsDetails != null)
                                {
                                    if (switchToNewerVersion)
                                    {
                                        subLevelDetails.ColumnName = finalSubLevelsDetails.DestinationColumnName;
                                        subLevelDetails.TableId = finalSubLevelsDetails.DestinationTableId;
                                    }
                                    else
                                    {
                                        subLevelDetails.ColumnName = finalSubLevelsDetails.SourceColumnName;
                                        subLevelDetails.TableId = await GetTableId(finalSubLevelsDetails.SourceColumnName, finalSubLevelsDetails.SourceTableId, duplicates, subLevelDetails.Settings.Select(x => x.MetricColumnName).ToList());
                                    }
                                }

                                foreach (var subSet in subLevelDetails.Settings)
                                {
                                    var finalSubSettingsInflightOverlayDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subSet.InflightOverlayTableId && x.SourceColumnName == subSet.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subSet.InflightOverlayTableId && x.DestinationColumnName == subSet.InflightOverlayColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                    if (finalSubSettingsInflightOverlayDetails != null)
                                    {
                                        if (switchToNewerVersion)
                                        {
                                            subSet.InflightOverlayColumnName = finalSubSettingsInflightOverlayDetails.DestinationColumnName;
                                            subSet.InflightOverlayTableId = finalSubSettingsInflightOverlayDetails.DestinationTableId;
                                        }
                                        else
                                        {
                                            subSet.InflightOverlayColumnName = finalSubSettingsInflightOverlayDetails.SourceColumnName;
                                            subSet.InflightOverlayTableId = await GetTableId(finalSubSettingsInflightOverlayDetails.SourceColumnName, finalSubSettingsInflightOverlayDetails.SourceTableId, duplicates);
                                        }
                                    }

                                    var finalSubSettingsMetricDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subSet.MetricTableId && x.SourceColumnName == subSet.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subSet.MetricTableId && x.DestinationColumnName == subSet.MetricColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                    if (finalSubSettingsMetricDetails != null)
                                    {
                                        if (switchToNewerVersion)
                                        {
                                            subSet.MetricColumnName = finalSubSettingsMetricDetails.DestinationColumnName;
                                            subSet.MetricTableId = finalSubSettingsMetricDetails.DestinationTableId;
                                        }
                                        else
                                        {
                                            subSet.MetricColumnName = finalSubSettingsMetricDetails.SourceColumnName;
                                            subSet.MetricTableId = await GetTableId(finalSubSettingsMetricDetails.SourceColumnName, finalSubSettingsMetricDetails.SourceTableId, duplicates);
                                        }
                                    }

                                    if (subSet.SubTotals != null)
                                    {
                                        foreach (var subLevelSubTotal in subSet.SubTotals)
                                        {
                                            var finalSubLevelSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelSubTotal.TableId && x.SourceColumnName == subLevelSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelSubTotal.TableId && x.DestinationColumnName == subLevelSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                            if (finalSubLevelSubTotalDetails != null)
                                            {
                                                if (switchToNewerVersion)
                                                {
                                                    subLevelSubTotal.ColumnName = finalSubLevelSubTotalDetails.DestinationColumnName;
                                                    subLevelSubTotal.TableId = finalSubLevelSubTotalDetails.DestinationTableId;
                                                }
                                                else
                                                {
                                                    subLevelSubTotal.ColumnName = finalSubLevelSubTotalDetails.SourceColumnName;
                                                    subLevelSubTotal.TableId = await GetTableId(finalSubLevelSubTotalDetails.SourceColumnName, finalSubLevelSubTotalDetails.SourceTableId, duplicates);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (subLevelDetails.SubTotals != null)
                                {
                                    foreach (var subTotalLevels in subLevelDetails.SubTotals)
                                    {
                                        var finalSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subTotalLevels.TableId && x.SourceColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subTotalLevels.TableId && x.DestinationColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                        if (finalSubTotalDetails != null)
                                        {
                                            if (switchToNewerVersion)
                                            {
                                                subTotalLevels.ColumnName = finalSubTotalDetails.DestinationColumnName;
                                                subTotalLevels.TableId = finalSubTotalDetails.DestinationTableId;
                                            }
                                            else
                                            {
                                                subTotalLevels.ColumnName = finalSubTotalDetails.SourceColumnName;
                                                subTotalLevels.TableId = await GetTableId(finalSubTotalDetails.SourceColumnName, finalSubTotalDetails.SourceTableId, duplicates);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (set.SubTotals != null)
                        {
                            foreach (var subLevelsSubTotal in set.SubTotals)
                            {
                                var subLevelsSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subLevelsSubTotal.TableId && x.SourceColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subLevelsSubTotal.TableId && x.DestinationColumnName == subLevelsSubTotal.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                                if (subLevelsSubTotalDetails != null)
                                {
                                    if (switchToNewerVersion)
                                    {
                                        subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.DestinationColumnName;
                                        subLevelsSubTotal.TableId = subLevelsSubTotalDetails.DestinationTableId;
                                    }
                                    else
                                    {
                                        subLevelsSubTotal.ColumnName = subLevelsSubTotalDetails.SourceColumnName;
                                        subLevelsSubTotal.TableId = await GetTableId(subLevelsSubTotalDetails.SourceColumnName, subLevelsSubTotalDetails.SourceTableId, duplicates);
                                    }
                                }
                            }
                        }
                    }

                    if (lvl.SubTotals != null)
                    {
                        foreach (var subTotalLevels in lvl.SubTotals)
                        {
                            var finalSubTotalDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == subTotalLevels.TableId && x.SourceColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == subTotalLevels.TableId && x.DestinationColumnName == subTotalLevels.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                            if (finalSubTotalDetails != null)
                            {
                                if (switchToNewerVersion)
                                {
                                    subTotalLevels.ColumnName = finalSubTotalDetails.DestinationColumnName;
                                    subTotalLevels.TableId = finalSubTotalDetails.DestinationTableId;
                                }
                                else
                                {
                                    subTotalLevels.ColumnName = finalSubTotalDetails.SourceColumnName;
                                    subTotalLevels.TableId = await GetTableId(finalSubTotalDetails.SourceColumnName, finalSubTotalDetails.SourceTableId, duplicates);
                                }
                            }
                        }
                    }
                }
            }
            return mediaHierarchyLevelDefinition;
        }

        /// <summary>
        /// Updates the run restrictions definition.
        /// </summary>
        /// <param name="definition">The RunRestriction definition</param>
        /// <param name="switchToNewerVersion">Switch to new version</param>
        /// <param name="mappingDetails">The mapping details</param>
        /// <param name="duplicates">The duplicates</param>
        /// <param name="omniClientId">OmniClientId</param>
        public async Task<ICollection<RunRestriction>> UpdateRunRestrictionsDefinition(ICollection<RunRestriction>? definition, bool switchToNewerVersion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, Guid? omniClientId = null)
        {
            if (definition != null)
            {
                foreach (var runRestriction in definition)
                {
                    var finalRunRestrictionDetails = switchToNewerVersion ? mappingDetails.FirstOrDefault(x => x.SourceTableId == runRestriction.TableId && x.SourceColumnName == runRestriction.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null)) : mappingDetails.FirstOrDefault(x => x.DestinationTableId == runRestriction.TableId && x.DestinationColumnName == runRestriction.ColumnName && (x.OmniClientId == omniClientId || x.OmniClientId == null));
                    if (finalRunRestrictionDetails != null)
                    {
                        if (switchToNewerVersion)
                        {
                            runRestriction.ColumnName = finalRunRestrictionDetails.DestinationColumnName;
                            runRestriction.TableId = finalRunRestrictionDetails.DestinationTableId;
                        }
                        else
                        {
                            runRestriction.ColumnName = finalRunRestrictionDetails.SourceColumnName;
                            runRestriction.TableId = await GetTableId(finalRunRestrictionDetails.SourceColumnName, finalRunRestrictionDetails.SourceTableId, duplicates);
                        }
                    }
                }
            }
            return definition;
        }

        /// <summary>
        /// Migrate client by Omni GUID.
        /// </summary>
        /// <param name="omniClientId">The OmniClientId</param>
        /// <param name="clientId">The ClientId</param>
        /// <param name="clientMappingDetails">The ClientMappindDetails</param>
        /// <param name="mappingDetails">The MappingDetails</param>
        /// <param name="duplicates">The Duplicates</param>
        /// <param name="cancellationToken">The cancellation token</param>
        public async Task MigrateClientByOmniGuid(Guid omniClientId, Guid clientId, ClientMapping clientMappingDetails, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var allClients = await portalDbContext.OmniClients.Where(x => x.ClientId == clientId).Select(c => c.Id).ToListAsync(cancellationToken);
            #region ThemeTemplates definition update start

            var themeTemplatesResult = await portalDbContext.ThemeTemplates.Where(x => allClients.Any(c => c == x.OmniClientId) && x.ThemeDefinition != null).ToListAsync();

            if (themeTemplatesResult.Count > 0)
            {
                foreach (var themes in themeTemplatesResult)
                {
                    var themeDefinitionDetailsDTO = new ThemeDefinitionDetailsDTO() { Definition = JsonSerializer.Deserialize<ThemeDefinition>(themes.ThemeDefinition, GetJsonSerializerOptions()), Id = themes.Id, CreatedByUserId = themes.CreatedByUserId };
                    await GetUpdatedThemeTemplateForPMDS(themeDefinitionDetailsDTO, themes, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                }
            }

            #endregion ThemeTemplates definition update end

            #region MediaHierarchyTemplates definition update start

            var mediaHierarchyTemplatesResult = await portalDbContext.MediaHierarchyTemplates.Where(x => allClients.Any(c => c == x.OmniClientId) && x.MediaHierarchyDefinition != null).ToListAsync(cancellationToken);

            if (mediaHierarchyTemplatesResult.Count > 0)
            {
                foreach (var res in mediaHierarchyTemplatesResult)
                {
                    // Query MediaHierarchy table to get the definition by ID
                    var mediaHierarchyDefinitionDetailsDTO = new MediaHierarchyDefinitionDetailsDTO() { Definition = JsonSerializer.Deserialize<MediaHierarchyDefinition>(res.MediaHierarchyDefinition, GetJsonSerializerOptions()), Id = res.Id, CreatedByUserId = res.CreatedByUserId };
                    await GetUpdatedMediaHierarchyForPMDS(mediaHierarchyDefinitionDetailsDTO, res, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                }
            }

            #endregion MediaHierarchyTemplates definition update end

            #region RightHandTotals definition update start

            var rightHandTotalsResult = await portalDbContext.TotalsTemplates.Where(x => allClients.Any(c => c == x.OmniClientId) && x.TotalsDefinition != null).ToListAsync(cancellationToken);

            if (rightHandTotalsResult.Count > 0)
            {
                foreach (var res in rightHandTotalsResult)
                {
                    // Query RightHandTotals table to get the definition by ID
                    var rightHandTotalsDefinitionDetailsDTO = new RightHandTotalsDefinitionDetailsDTO() { Definition = JsonSerializer.Deserialize<TotalsDefinition>(res.TotalsDefinition, GetJsonSerializerOptions()), Id = res.Id, CreatedByUserId = res.CreatedByUserId };
                    await GetUpdatedRightHandTotalsForPMDS(rightHandTotalsDefinitionDetailsDTO, res, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                }
            }

            #endregion RightHandTotals definition update end

            #region GrandTotalTemplates definition update start

            var grandTotalTemplatesResult = await portalDbContext.GrandTotalTemplates.Where(x => allClients.Any(c => c == x.OmniClientId) && x.GrandTotalDefinition != null).ToListAsync(cancellationToken);

            if (grandTotalTemplatesResult.Count > 0)
            {
                foreach (var res in grandTotalTemplatesResult)
                {
                    // Query GrandTotalTemplates table to get the definition by ID
                    var grandTotalDefintionDetailsDTO = new GrandTotalDefintionDetailsDTO() { Definition = JsonSerializer.Deserialize<GrandTotalDefinition>(res.GrandTotalDefinition, GetJsonSerializerOptions()), Id = res.Id, CreatedByUserId = res.CreatedByUserId };
                    await GetUpdatedGrandTotalForPMDS(grandTotalDefintionDetailsDTO, res, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                }
            }

            #endregion GrandTotalTemplates definition update end

            #region HeaderTemplates definition update start 

            var headerTemplatesResult = await portalDbContext.HeaderTemplates.Where(x => allClients.Any(c => c == x.OmniClientId) && x.HeaderDefinition != null).ToListAsync(cancellationToken);

            if (headerTemplatesResult.Count > 0)
            {
                foreach (var res in headerTemplatesResult)
                {
                    // Query HeaderTemplates table to get the definition by ID
                    var headerDefinitionDetailsDTO = new HeaderDefinitionDetailsDTO() { Definition = JsonSerializer.Deserialize<HeaderDefinition>(res.HeaderDefinition, GetJsonSerializerOptions()), Id = res.Id, CreatedByUserId = res.CreatedByUserId };
                    await GetUpdatedHeaderForPMDS(headerDefinitionDetailsDTO, res, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                }
            }

            #endregion HeaderTemplates definition update end

            // Changes require in FlowchartTemplates, FlowchartTemplatesVersionHistorties, RunConfiguration defintions
            var flowchartTemplates = await portalDbContext.FlowchartTemplates.Where(a => allClients.Any(c => c == a.OmniClientId) && a.FlowchartDefinition != null).ToListAsync(cancellationToken);
            if (flowchartTemplates.Count > 0)
            {
                foreach (var flowchartTemplate in flowchartTemplates)
                {
                    #region FlowchartTemplates definition update start

                    if (flowchartTemplate.FlowchartDefinition != null)
                    {
                        // Query FlowchartTemplates table to get the definition by ID
                        var flowchartDefinitionDetailsDTO = new FlowchartDefinitionDetailsDTO() { Definition = JsonSerializer.Deserialize<FlowchartDefinition>(flowchartTemplate.FlowchartDefinition, GetJsonSerializerOptions()), Id = flowchartTemplate.Id, CreatedByUserId = flowchartTemplate.CreatedByUserId };
                        await GetUpdatedFlowchartForPMDS(flowchartDefinitionDetailsDTO, flowchartTemplate, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                    }

                    #endregion FlowchartTemplates definition update end

                    #region FlowchartTemplatesVersionHistorties definition update start

                    var flowchartTemplateVersionHistorties = flowchartTemplate.FlowchartTemplatesVersionHistorties.Where(x=> x.FlowchartDefinition != null).ToList();

                    foreach (var flowchartTemplateVersionHistorty in flowchartTemplateVersionHistorties)
                    {
                        if (flowchartTemplateVersionHistorty.FlowchartDefinition != null)
                        {
                            // Query FlowchartTemplatesVersionHistorties table to get the definition by ID
                            var flowchartTemplatesVersionHistortiesDefinitionDetailsDTO = new FlowchartTemplatesVersionHistortiesDefinitionDetailsDTO() { Definition = JsonSerializer.Deserialize<FlowchartDefinition>(flowchartTemplateVersionHistorty.FlowchartDefinition, GetJsonSerializerOptions()), Id = flowchartTemplateVersionHistorty.Id, CreatedByUserId = flowchartTemplateVersionHistorty.CreatedByUserId };
                            await GetUpdatedFlowchartTemplatesVersionHistortiesForPMDS(flowchartTemplatesVersionHistortiesDefinitionDetailsDTO, flowchartTemplateVersionHistorty, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                        }
                    }

                    #endregion FlowchartTemplatesVersionHistorties definition update end

                    #region runcConfiguration definition update start

                    var runConfigurations = flowchartTemplate.RunConfigurations.ToList();

                    foreach (var runConfiguration in runConfigurations)
                    {
                        // Query RunConfiguration table to get the definition by ID
                        var runDetails = new RunConfigurationDefinitionDetailsDTO()
                        {
                            Id = runConfiguration.Id,
                            MediaHierarchyLevels = runConfiguration.MediaHierarchyLevels == null ? null : JsonSerializer.Deserialize<ICollection<MediaHierarchyLevelBase>>(runConfiguration.MediaHierarchyLevels, GetJsonSerializerOptions()),
                            RunRestrictions = runConfiguration.RunRestrictions == null ? null : JsonSerializer.Deserialize<ICollection<RunRestriction>>(runConfiguration.RunRestrictions, GetJsonSerializerOptions()),
                            CreatedByUserId = runConfiguration.CreatedByUserId,
                        };
                        await GetUpdatedRunConfigurationForPMDS(runDetails, runConfiguration, clientMappingDetails.Version, mappingDetails, duplicates, cancellationToken, omniClientId);
                    }

                    #endregion runcConfiguration definition update end

                }
            }

            log.Add(LogLevel.Information, $"Migration done with background job for omniClientID: {omniClientId} .");
        }

        /// <summary>
        /// Gets the Media Hierarchy columns and tables details for PMDS.
        /// </summary>
        /// <param name="mediaHierarchyDefinitionWithOmniClientId">The mediaHierarchyDefinitionWithOmniClientId.</param>
        /// <param name="omniClientIdWithClientIdList">The distinctOmniGuids</param>
        /// <param name="clientColumnDetail">The clientColumnDetail</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        public async Task<MediaHierarchyColumnsDetailsResultDTO> GetMediaHierarchyColumnsUsedDetails(List<MediaHierarchyDefinitionWithOmniClientId> mediaHierarchyDefinitionWithOmniClientId, List<OmniClientIdWithClientId>? omniClientIdWithClientIdList, List<MediaHierarchyColumnsDetailss> clientColumnDetail, CancellationToken cancellationToken)
        {
            bool isActualColumnsRequires = true;
            List<MediaHierarchyColumnsDetailsDTO> actualMediaHierarchyColumnsDetails = new List<MediaHierarchyColumnsDetailsDTO>();
            List<MediaHierarchyColumnsDetailsDTO> differentMediaHierarchyColumnsDetails = new List<MediaHierarchyColumnsDetailsDTO>();

            foreach (var omniClientGuid in omniClientIdWithClientIdList)
            {
                List<MediaHierarchyColumnAndTableDetails> mediaHierarchyColumnAndTableDetails = new List<MediaHierarchyColumnAndTableDetails>();
                var mediaHierarchyDefinitionList = mediaHierarchyDefinitionWithOmniClientId.Where(x => x.OmniClientId == omniClientGuid.OmniClientId).Select(x => x.Definition).ToList();

                if (mediaHierarchyDefinitionList != null && mediaHierarchyDefinitionList.Count > 0)
                {
                    foreach (var definition in mediaHierarchyDefinitionList)
                    {
                        if (definition != null)
                        {
                            foreach (var lvl in definition.Levels)
                            {
                                if (lvl.TableId != null && lvl.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == lvl.TableId && x.ColumnName == lvl.ColumnName))
                                {
                                    var level = new MediaHierarchyColumnAndTableDetails
                                    {
                                        TableId = lvl.TableId,
                                        ColumnName = lvl.ColumnName,
                                    };
                                    mediaHierarchyColumnAndTableDetails.Add(level);
                                }

                                foreach (var set in lvl.Settings)
                                {
                                    if (set.InflightOverlayTableId != null && set.InflightOverlayColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == set.InflightOverlayTableId && x.ColumnName == set.InflightOverlayColumnName))
                                    {
                                        var lvlSettingsInflightOverlay = new MediaHierarchyColumnAndTableDetails
                                        {
                                            TableId = set.InflightOverlayTableId,
                                            ColumnName = set.InflightOverlayColumnName,
                                        };
                                        mediaHierarchyColumnAndTableDetails.Add(lvlSettingsInflightOverlay);
                                    }

                                    if (set.MetricTableId != null && set.MetricColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == set.MetricTableId && x.ColumnName == set.MetricColumnName))
                                    {
                                        var lvlSettingsMetric = new MediaHierarchyColumnAndTableDetails
                                        {
                                            TableId = set.MetricTableId,
                                            ColumnName = set.MetricColumnName,
                                        };
                                        mediaHierarchyColumnAndTableDetails.Add(lvlSettingsMetric);
                                    }

                                    if (set.SubLevels != null)
                                    {
                                        foreach (var subLevelDetails in set.SubLevels)
                                        {
                                            if (subLevelDetails.TableId != null && subLevelDetails.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subLevelDetails.TableId && x.ColumnName == subLevelDetails.ColumnName))
                                            {
                                                var subLevels = new MediaHierarchyColumnAndTableDetails
                                                {
                                                    TableId = subLevelDetails.TableId,
                                                    ColumnName = subLevelDetails.ColumnName,
                                                };
                                                mediaHierarchyColumnAndTableDetails.Add(subLevels);
                                            }

                                            foreach (var subSet in subLevelDetails.Settings)
                                            {

                                                if (subSet.InflightOverlayTableId != null && subSet.InflightOverlayColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subSet.InflightOverlayTableId && x.ColumnName == subSet.InflightOverlayColumnName))
                                                {
                                                    var subSetInflightOverlay = new MediaHierarchyColumnAndTableDetails
                                                    {
                                                        TableId = subSet.InflightOverlayTableId,
                                                        ColumnName = subSet.InflightOverlayColumnName,
                                                    };
                                                    mediaHierarchyColumnAndTableDetails.Add(subSetInflightOverlay);
                                                }

                                                if (subSet.MetricTableId != null && subSet.MetricColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subSet.MetricTableId && x.ColumnName == subSet.MetricColumnName))
                                                {
                                                    var subSetMetric = new MediaHierarchyColumnAndTableDetails
                                                    {
                                                        TableId = subSet.MetricTableId,
                                                        ColumnName = subSet.MetricColumnName,
                                                    };
                                                    mediaHierarchyColumnAndTableDetails.Add(subSetMetric);
                                                }

                                                if (subSet.SubTotals != null)
                                                {
                                                    foreach (var subLevelSubTotal in subSet.SubTotals)
                                                    {
                                                        if (subLevelSubTotal.TableId != null && subLevelSubTotal.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subLevelSubTotal.TableId && x.ColumnName == subLevelSubTotal.ColumnName))
                                                        {
                                                            var subLevelsSubTotal = new MediaHierarchyColumnAndTableDetails
                                                            {
                                                                TableId = subLevelSubTotal.TableId,
                                                                ColumnName = subLevelSubTotal.ColumnName,
                                                            };
                                                            mediaHierarchyColumnAndTableDetails.Add(subLevelsSubTotal);
                                                        }
                                                    }
                                                }
                                            }

                                            if (subLevelDetails.SubTotals != null)
                                            {
                                                foreach (var subTotalLevels in subLevelDetails.SubTotals)
                                                {

                                                    if (subTotalLevels.TableId != null && subTotalLevels.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subTotalLevels.TableId && x.ColumnName == subTotalLevels.ColumnName))
                                                    {
                                                        var subTotals = new MediaHierarchyColumnAndTableDetails
                                                        {
                                                            TableId = subTotalLevels.TableId,
                                                            ColumnName = subTotalLevels.ColumnName,
                                                        };
                                                        mediaHierarchyColumnAndTableDetails.Add(subTotals);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (set.SubTotals != null)
                                    {
                                        foreach (var subLevelsSubTotal in set.SubTotals)
                                        {
                                            if (subLevelsSubTotal.TableId != null && subLevelsSubTotal.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subLevelsSubTotal.TableId && x.ColumnName == subLevelsSubTotal.ColumnName))
                                            {
                                                var setSubLevelsSubTotal = new MediaHierarchyColumnAndTableDetails
                                                {
                                                    TableId = subLevelsSubTotal.TableId,
                                                    ColumnName = subLevelsSubTotal.ColumnName,
                                                };
                                                mediaHierarchyColumnAndTableDetails.Add(setSubLevelsSubTotal);
                                            }
                                        }
                                    }

                                    if (set.SubTotalSummary != null)
                                    {
                                        foreach (var subTotalSummary in set.SubTotalSummary)
                                        {
                                            foreach (var subTotalSummarySubTotals in subTotalSummary.SubTotals)
                                            {
                                                if (subTotalSummarySubTotals.TableId != null && subTotalSummarySubTotals.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subTotalSummarySubTotals.TableId && x.ColumnName == subTotalSummarySubTotals.ColumnName))
                                                {
                                                    var subLevelsSummarySubTotals = new MediaHierarchyColumnAndTableDetails
                                                    {
                                                        TableId = subTotalSummarySubTotals.TableId,
                                                        ColumnName = subTotalSummarySubTotals.ColumnName,
                                                    };
                                                    mediaHierarchyColumnAndTableDetails.Add(subLevelsSummarySubTotals);
                                                }
                                            }

                                            foreach (var subTotalSummarySettings in subTotalSummary.Settings)
                                            {
                                                if (subTotalSummarySettings.TableId != null && subTotalSummarySettings.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subTotalSummarySettings.TableId && x.ColumnName == subTotalSummarySettings.ColumnName))
                                                {
                                                    var subTotalsSummarySettings = new MediaHierarchyColumnAndTableDetails
                                                    {
                                                        TableId = subTotalSummarySettings.TableId,
                                                        ColumnName = subTotalSummarySettings.ColumnName,
                                                    };
                                                    mediaHierarchyColumnAndTableDetails.Add(subTotalsSummarySettings);
                                                }
                                            }
                                        }
                                    }

                                    if (set.InflightOverlays != null)
                                    {
                                        foreach (var inflightOverlays in set.InflightOverlays)
                                        {
                                            if (inflightOverlays.TableId != null && inflightOverlays.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == inflightOverlays.TableId && x.ColumnName == inflightOverlays.ColumnName))
                                            {
                                                var inflightOverlay = new MediaHierarchyColumnAndTableDetails
                                                {
                                                    TableId = inflightOverlays.TableId,
                                                    ColumnName = inflightOverlays.ColumnName,
                                                };
                                                mediaHierarchyColumnAndTableDetails.Add(inflightOverlay);
                                            }
                                        }
                                    }
                                }

                                if (lvl.SubTotals != null)
                                {
                                    foreach (var subTotalLevels in lvl.SubTotals)
                                    {
                                        if (subTotalLevels.TableId != null && subTotalLevels.ColumnName != null && !mediaHierarchyColumnAndTableDetails.Any(x => x.TableId == subTotalLevels.TableId && x.ColumnName == subTotalLevels.ColumnName))
                                        {
                                            var subTotals = new MediaHierarchyColumnAndTableDetails
                                            {
                                                TableId = subTotalLevels.TableId,
                                                ColumnName = subTotalLevels.ColumnName,
                                            };
                                            mediaHierarchyColumnAndTableDetails.Add(subTotals);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var clientInfos = new MediaHierarchyColumnsDetailsDTO
                {
                    OmniClientId = omniClientGuid.OmniClientId,
                    ClientName = omniClientGuid.ClientName,
                    ClientId = omniClientGuid.ClientId,
                    ColumnInfos = mediaHierarchyColumnAndTableDetails,
                };
                actualMediaHierarchyColumnsDetails.Add(clientInfos);
            }
            // Clone the list so filtering doesn't mutate the original
            var allColumns = actualMediaHierarchyColumnsDetails
                .Select(x => new MediaHierarchyColumnsDetailsDTO
                {
                    OmniClientId = x.OmniClientId,
                    ClientName = x.ClientName,
                    ClientId = x.ClientId,
                    ColumnInfos = x.ColumnInfos != null
                        ? x.ColumnInfos.Select(ci => new MediaHierarchyColumnAndTableDetails
                        {
                            TableId = ci.TableId,
                            ColumnName = ci.ColumnName
                        }).ToList()
                        : new List<MediaHierarchyColumnAndTableDetails>(),
                })
                .GroupBy(x => x.OmniClientId)
                .Select(g => g.First())
                .OrderBy(x => x.ClientName)
                .ToList();

            var data = new MediaHierarchyColumnsDetailsResultDTO();
            data.AllColumns = allColumns;

            var differentDetails = (await CompareMediaHierarchyColumnsDetailsAsync(
                    allColumns.Select(x => new MediaHierarchyColumnsDetailsDTO
                    {
                        OmniClientId = x.OmniClientId,
                        ClientName = x.ClientName,
                        ClientId = x.ClientId,
                        ColumnInfos = x.ColumnInfos != null
                            ? x.ColumnInfos.Select(ci => new MediaHierarchyColumnAndTableDetails
                            {
                                TableId = ci.TableId,
                                ColumnName = ci.ColumnName
                            }).ToList()
                            : new List<MediaHierarchyColumnAndTableDetails>(),
                    }).ToList(), clientColumnDetail))
                .GroupBy(x => x.OmniClientId)
                .Select(g => g.First())
                .ToList();

            data.FilteredColumns = differentDetails;

            return data;

        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<MediaHierarchyColumnsDetailsDTO>> CompareMediaHierarchyColumnsDetailsAsync(List<MediaHierarchyColumnsDetailsDTO> mediaHierarchyColumnsDetails, List<MediaHierarchyColumnsDetailss> clientColumnDetail)
        {
            var result = new List<MediaHierarchyColumnsDetailsDTO>();

            foreach (var mh in mediaHierarchyColumnsDetails)
            {
                var amdColumns = clientColumnDetail.Where(ac => mh.ClientName != null && ac.ClientName != null && mh.OmniClientId != null && ac.OmniClientId != null && mh.ClientName.ToUpper() == ac.ClientName.ToUpper() && mh.OmniClientId.ToString().ToUpper() == ac.OmniClientId.ToString().ToUpper()).SelectMany(z => z.ColumnInfoss).ToList();

                mh.ColumnInfos = mh.ColumnInfos.Where(x => !amdColumns.Any(ac => ac.ColumnName.ToUpper() == x.ColumnName.ToUpper())).ToList();

                result.Add(mh);
            }

            return result;
        }
    }
}
