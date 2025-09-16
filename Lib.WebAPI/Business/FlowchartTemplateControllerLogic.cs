using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Hangfire;
using JasperFx.Core;
using Lib.Annalect.Business;
using Lib.Athena.Consts;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Enums;
using Lib.WebAPI.Extensions;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;
using Newtonsoft.Json;
using NSubstitute;
using OneOf.Types;
using Polly;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Threading;
using Throw;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using User = Lib.WebAPI.DbModels.User;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// FlowchartTemplateControllerLogic
    /// </summary>
    public class FlowchartTemplateControllerLogic : IFlowchartTemplateControllerLogic
    {
        private static readonly Guid MediaPlansTableId = Guid.Parse("F0867476-5D91-4FBF-8EBF-C5308309F4EA");
        private static readonly Guid MediaBriefsTableId = Guid.Parse("3E185D41-9E21-44DC-9532-CD4BDA1B5A0D");
        private const string ExcelFileExtension = ".xlsx";
        private const string DataFileExtension = ".data";
        private const string FlowchartTemplateFileExtension = ".template";
        private const string RunConfigFileExtension = ".settings";
        private readonly ILog<IFlowchartTemplateControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly IPortalDbContext portalDbContext;
        private readonly DataControllerLogicPmds dataControllerLogicPmds;
        private readonly DataControllerLogic controllerLogic;
        private readonly IClientControllerLogic clientControllerLogic;
        private static readonly Guid DefaultMetricTableId = Guid.Parse("f0867476-5d91-4fbf-8ebf-c5308309f4ea");
        private static readonly Guid DefaultMetricTableIdPMDS = Guid.Parse("0d771867-cc9a-40a5-900e-f57528f91d0e");
        private static readonly string DefaultMetricColumnName = "netmedia";
        private readonly AzureBlobStorage storage;
        private readonly IOptionsMonitor<StorageConfig> storageConfig;
        private readonly IOptionsMonitor<DefaultTemplateUpdaterConfig> defaultTemplateConfig;
        private readonly IMigrationControllerLogic migrationControllerLogic;

        private readonly Dictionary<Type, Action<FlowchartDefinition, FlowchartDefinition, NamedDbModelBase>> updateTemplateActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplateControllerLogic"
        /// /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portalDbContext</param>
        /// <param name="annalectApiClient">The annalectApiClient</param>
        /// <param name="dataControllerLogicPmds">The dataControllerLogicPmds</param>
        /// <param name="controllerLogic">The controllerLogic</param>
        /// <param name="clientControllerLogic">The clientControllerLogic</param>"
        /// <param name="storage">The storage.</param>
        /// <param name="storageConfig">The storage configuration.</param>
        /// <param name="defaultTemplateConfig">The default template configuration.</param>
        /// <param name="migrationControllerLogic">The migration controller logic.</param>
        public FlowchartTemplateControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<IFlowchartTemplateControllerLogic> log,
            IPortalDbContext portalDbContext,
            DataControllerLogicPmds dataControllerLogicPmds,
            DataControllerLogic controllerLogic,
            IClientControllerLogic clientControllerLogic,
            AzureBlobStorage storage,
            IOptionsMonitor<StorageConfig> storageConfig,
            IOptionsMonitor<DefaultTemplateUpdaterConfig> defaultTemplateConfig,
            IMigrationControllerLogic migrationControllerLogic)
        {
            this.portal = portal;
            this.mapper = mapper;
            this.securityLogic = securityLogic;
            this.log = log;
            this.portalDbContext = portalDbContext;
            this.dataControllerLogicPmds = dataControllerLogicPmds;
            this.controllerLogic = controllerLogic;
            this.clientControllerLogic = clientControllerLogic;
            this.storage = storage;
            this.storageConfig = storageConfig;
            this.defaultTemplateConfig = defaultTemplateConfig;
            updateTemplateActions = new Dictionary<Type, Action<FlowchartDefinition, FlowchartDefinition, NamedDbModelBase>>
        {
            { typeof(HeaderTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateHeaderTemplate(defaultFlowchart, currentFlowchart, (HeaderTemplate)template) },
            { typeof(CalendarTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateCalendarTemplate(defaultFlowchart, currentFlowchart, (CalendarTemplate)template) },
            { typeof(CalendarOverlayTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateCalendarOverlayTemplate(defaultFlowchart, currentFlowchart, (CalendarOverlayTemplate)template) },
            { typeof(MediaHierarchyTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateMediaHierarchyTemplate(defaultFlowchart, currentFlowchart, (MediaHierarchyTemplate)template) },
            { typeof(GrandTotalTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateGrandTotalTemplate(defaultFlowchart, currentFlowchart, (GrandTotalTemplate)template) },
            { typeof(TotalsTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateTotalsTemplate(defaultFlowchart, currentFlowchart, (TotalsTemplate)template) },
            { typeof(ThemeTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateThemeTemplate(defaultFlowchart, currentFlowchart, (ThemeTemplate)template) },
            { typeof(FooterTemplate), (defaultFlowchart, currentFlowchart, template) => UpdateFooterTemplate(defaultFlowchart, currentFlowchart, (FooterTemplate)template) },
        };
            this.migrationControllerLogic = migrationControllerLogic;
        }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="flowchartTemplate">The flowchart template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateDetailsDTO> CreateAsync(
            FlowchartTemplateCreateDTO flowchartTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(flowchartTemplate.OmniClientId, user.Id, cancellationToken);

            if (await portal.FlowchartTemplates.AnyAsync(
                cancellationToken,
                x => x.Name == flowchartTemplate.TemplateName && x.OmniClientId == flowchartTemplate.OmniClientId))
            {
                throw new ArgumentException($"Template already exists with this name.");
            }

            var template = new FlowchartTemplate
            {
                Name = flowchartTemplate.TemplateName,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                OmniClientId = flowchartTemplate.OmniClientId,
                FlowchartDefinition = flowchartTemplate.Definition == null ? null : Json.Serialize(flowchartTemplate.Definition),
            };

            log.Add(LogLevel.Information, $"Creating flowchart template: {template.Id} by user: {template.CreatedByUserId}");

            template = await portal.FlowchartTemplates.AddAsync(template, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<FlowchartTemplateDetailsDTO>(template);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.SoftDeleteAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
            template.ModifiedDate = DateTime.UtcNow;
            log.Add(LogLevel.Information, $"Deleting flowchart template: {template.Id} by user: {userId}");

            await portal.SaveChangesAsync(cancellationToken);
            return mapper.Map<FlowchartTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Restores the asynchronous.
        /// </summary>
        /// <param name="id">The identifier of the template to restore.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.SoftRestoreAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);
            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
            template.ModifiedDate = DateTime.UtcNow;
            log.Add(LogLevel.Information, $"Restoring flowchart template: {template.Id} by user: {userId}");

            await portal.SaveChangesAsync(cancellationToken);
            return mapper.Map<FlowchartTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The named search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateInfoListDTO> GetAsync(FlowchartTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(search.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.FlowchartTemplates.SearchAllAsync<FlowchartTemplateInfoDTO>(
                search, cancellationToken, x => x.OmniClientId == search.OmniClientId && x.Removed == search.Removed); // To include isRemoved check

            foreach (var item in items)
            {
                int reportCount = portalDbContext.Reports.Where(x => x.FlowchartTemplateId == item.Id).Count();
                int runConfiguationCount = portalDbContext.RunConfigurations.Where(x => x.FlowchartTemplateId == item.Id).Count();
                item.IsReportAvailable = reportCount > 0 ? true : false;
                item.IsRunConfigurationAvailable = runConfiguationCount > 0 ? true : false;

                item.ShareStatus = item.ParentId.HasValue
                ? TemplateShareStatus.Received
                : await portal.FlowchartTemplates.AnyAsync(cancellationToken, t => t.ParentId == item.Id)
                ? TemplateShareStatus.Shared
                : TemplateShareStatus.None;
            }
            items = items.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.CreatedDate).ToList();
            return new FlowchartTemplateInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Gets the flowchart template history details asynchronous.
        /// </summary>
        /// <param name="search">The named search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateDetailsDTO> SearchAsync(FlowchartTemplateVersionSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            var data = await portalDbContext.FlowchartTemplatesVersionHistorties.FirstOrDefaultAsync(x => x.FlowchartTemplateId == search.FlowchartTemplateId && x.Version == search.Version);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var result = await GetAsync(data.FlowchartTemplateId, userId, cancellationToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId, userId, cancellationToken);

            result.Version = search.Version;
            result.Name = data.Name;
#pragma warning disable CS8604 // Possible null reference argument.
            result.Definition = JsonConvert.DeserializeObject<FlowchartDefinition>(data.FlowchartDefinition);
#pragma warning restore CS8604 // Possible null reference argument.
            result.Comments = data.Comments;
            result.CustomStyleSettings = data.CustomStyleSettings;
            result.TrackFormatChanges = data.TrackFormatChanges;
            return result;

        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.FlowchartTemplates.GetByIdAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId, userId, cancellationToken);
            result.Version = result.FlowchartTemplatesVersionHistorties.Count < 1 ? 0 : result.Version;
            var dto = mapper.Map<FlowchartTemplateDetailsDTO>(result);

            // Migrate Inflight Overlay to List
            dto.Definition?.MediaHierarchyDefinition?.Definition?.Levels?.ToList().ForEach(mh =>
            {
                mh?.Settings?.ToList().ForEach(s =>
                {
                    s.MigrateInflightOverlayToList();
                });
            });
            return dto;
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateDetailsDTO> UpdateAsync(
            FlowchartTemplateUpdateDTO update, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.GetByIdAsync(update.Id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            if (await portal.FlowchartTemplates.AnyAsync(
                cancellationToken,
                x => x.Id != template.Id && x.Name == update.Name && x.OmniClientId == template.OmniClientId))
            {
                throw new ArgumentException($"Template already exists with this name.");
            }

            log.Add(LogLevel.Information, $"Updating flowchart template: {template.Id} by user: {template.ModifiedByUserId}");

            template = mapper.Map(update, template);

            var user = await portal.Users.GetByIdAsync(userId, cancellationToken);

            template.ModifiedByUser = user;
            template.ModifiedByUserId = user.Id;
            template.ModifiedDate = DateTime.UtcNow;
            if (template.FlowchartTemplatesVersionHistorties.Any())
            {
                template.Version++;
            }


            var editVersionHistory = await portal.FlowchartTemplatesVersionHistorties.AddAsync(
            new FlowchartTemplatesVersionHistorty
            {
                Name = template.Name,
                FlowchartTemplateId = template.Id,
                FlowchartDefinition = template.FlowchartDefinition,
                Version = template.Version,
                CreatedByUserId = template.CreatedByUserId,
                CreatedDate = DateTime.UtcNow,
                ModifiedByUserId = template.ModifiedByUserId,
                ModifiedDate = DateTime.UtcNow,
                ModifiedByUser = template.ModifiedByUser,
                CreatedByUser = template.CreatedByUser,
                FlowchartTemplate = template,
                Comments = update.Comments ?? "Initial Version",
                CustomStyleSettings = update.CustomStyleSettings,
                TrackFormatChanges = update.TrackFormatChanges,
            }, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return await GetAsync(template.Id, userId, cancellationToken);
        }

        /// <summary>
        /// Check the duplicate template name asynchronous.
        /// </summary>
        /// <param name="flowchartTemplate">The flowchart template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CheckDuplicateTemplateNameAsync(
            FlowchartTemplateDuplicateCheckDTO flowchartTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(flowchartTemplate.OmniClientId, user.Id, cancellationToken);

            if (await portal.FlowchartTemplates.AnyAsync(
                cancellationToken,
                x => x.Name == flowchartTemplate.TemplateName && x.OmniClientId == flowchartTemplate.OmniClientId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update the flowchart template name by using flowchart template Id asynchronous.
        /// </summary>
        /// <param name="updateflowchartTemplateName">The flowchart template.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> UpdateFlowChartTemplateNameByIdAsync(
            UpdateFlowchartTemplateNameDto updateflowchartTemplateName, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(updateflowchartTemplateName.OmniClientId, userId, cancellationToken);

            if (await portal.FlowchartTemplates.AnyAsync(
                cancellationToken,
                x => x.Id == updateflowchartTemplateName.FlowchartTemplateId && x.OmniClientId == updateflowchartTemplateName.OmniClientId))
            {
                var template = await portal.FlowchartTemplates.GetByIdAsync(updateflowchartTemplateName.FlowchartTemplateId, cancellationToken);
                securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
                template.Name = updateflowchartTemplateName.TemplateName;
                template.ModifiedDate = DateTime.UtcNow;
                template.ModifiedByUserId = userId;
                await portal.SaveChangesAsync(cancellationToken);
                log.Add(LogLevel.Information, $"Flowchart template name updated successfully with FlowchartTemplateId: {updateflowchartTemplateName.FlowchartTemplateId} by user: {userId}");
                return "Flowchart template name updated successfully.";
            }
            else
            {
                log.Add(LogLevel.Information, $"Flowchart template id not found with FlowchartTemplateId : {updateflowchartTemplateName.FlowchartTemplateId} by user: {userId}");
                return $"Flowchart template Id: {updateflowchartTemplateName.FlowchartTemplateId} is not found.";
            }
        }

        /// <summary>
        /// Get the flowchart template share info asynchronous.
        /// </summary>
        /// <param name="templateId">The templateId.</param>
        /// <param name="clientList">The clientList.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartTemplateShareInfoDTO> FlowchartTemplateShareInfoAsync(Guid templateId, List<ClientDetails> clientList, Guid userId, CancellationToken cancellationToken)
        {
            FlowchartTemplateShareInfoDTO flowchartTemplateComponentNamesWithClientListDTO = new FlowchartTemplateShareInfoDTO();
            List<ComponentDetails> cds = new List<ComponentDetails>();
            var flowchartTemplatesResult = await portal.FlowchartTemplates.GetByIdAsync(templateId, cancellationToken);

            flowchartTemplatesResult?.FlowchartDefinition.ThrowIfNull(nameof(flowchartTemplatesResult));

            FlowchartDefinition? flowchartDefinition = Json.Deserialize<FlowchartDefinition>(flowchartTemplatesResult.FlowchartDefinition);

            if (flowchartDefinition.HeaderDefinition != null)
            {
                var headerTemplateDetails = await portalDbContext.HeaderTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.HeaderDefinition.TemplateId && x.Removed == false);
                if (headerTemplateDetails != null)
                {
                    var ht = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.Header.ToString(),
                        Name = headerTemplateDetails.Name,
                        TemplateId = headerTemplateDetails.Id,
                    };
                    cds.Add(ht);
                }
            }

            if (flowchartDefinition.CalendarDefinition != null)
            {
                var calendarTemplateDetails = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarDefinition.TemplateId && x.Removed == false);
                if (calendarTemplateDetails != null)
                {
                    var cd = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.Calendar.ToString(),
                        Name = calendarTemplateDetails.Name,
                        TemplateId = calendarTemplateDetails.Id,
                    };
                    cds.Add(cd);
                }
            }

            if (flowchartDefinition.CalendarOverlayDefinition != null)
            {
                var calendarOverlayTemplateDetails = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarOverlayDefinition.TemplateId && x.Removed == false);
                if (calendarOverlayTemplateDetails != null)
                {
                    var co = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.CalendarOverlay.ToString(),
                        Name = calendarOverlayTemplateDetails.Name,
                        TemplateId = calendarOverlayTemplateDetails.Id,
                    };
                    cds.Add(co);
                }
            }

            if (flowchartDefinition.MediaHierarchyDefinition != null && flowchartDefinition.MediaHierarchyDefinition.Definition.Levels != null)
            {
                var mediaHierarchyTemplateDetails = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.MediaHierarchyDefinition.TemplateId && x.Removed == false);
                if (mediaHierarchyTemplateDetails != null)
                {
                    var mh = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.MediaHierarchy.ToString(),
                        Name = mediaHierarchyTemplateDetails.Name,
                        TemplateId = mediaHierarchyTemplateDetails.Id,
                    };
                    cds.Add(mh);
                }
            }

            if (flowchartDefinition.GrandTotalDefinition != null)
            {
                var grandTotalTemplateDetails = await portalDbContext.GrandTotalTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.GrandTotalDefinition.TemplateId && x.Removed == false);
                if (grandTotalTemplateDetails != null)
                {
                    var gt = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.GrandTotals.ToString(),
                        Name = grandTotalTemplateDetails.Name,
                        TemplateId = grandTotalTemplateDetails.Id,
                    };
                    cds.Add(gt);
                }
            }

            if (flowchartDefinition.TotalsDefinition != null)
            {
                var rightHandTotalsTemplateDetails = await portalDbContext.TotalsTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.TotalsDefinition.TemplateId && x.Removed == false);
                if (rightHandTotalsTemplateDetails != null)
                {
                    var rht = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.RightHandTotals.ToString(),
                        Name = rightHandTotalsTemplateDetails.Name,
                        TemplateId = rightHandTotalsTemplateDetails.Id,
                    };
                    cds.Add(rht);
                }
            }

            if (flowchartDefinition.ThemeDefinition != null)
            {
                var themeTemplateDetails = await portalDbContext.ThemeTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.ThemeDefinition.TemplateId && x.Removed == false);
                if (themeTemplateDetails != null)
                {
                    var tt = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.Theme.ToString(),
                        Name = themeTemplateDetails.Name,
                        TemplateId = themeTemplateDetails.Id,
                    };
                    cds.Add(tt);
                }
            }

            if (flowchartDefinition.FooterDefinition != null)
            {
                var footerTemplateDetails = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.FooterDefinition.TemplateId && x.Removed == false);
                if (footerTemplateDetails != null)
                {
                    var ft = new ComponentDetails()
                    {
                        ComponentName = FlowChartComponent.Footer.ToString(),
                        Name = footerTemplateDetails.Name,
                        TemplateId = footerTemplateDetails.Id,
                    };
                    cds.Add(ft);
                }
            }

            flowchartTemplateComponentNamesWithClientListDTO.TemplateId = flowchartTemplatesResult.Id;
            flowchartTemplateComponentNamesWithClientListDTO.TemplateName = flowchartTemplatesResult.Name;
            flowchartTemplateComponentNamesWithClientListDTO.ComponentDetailsList = cds;
            flowchartTemplateComponentNamesWithClientListDTO.ClientDetailsList = clientList;
            return flowchartTemplateComponentNamesWithClientListDTO;

        }

        /// <summary>
        /// Save flowchart share template asynchronous.
        /// </summary>
        /// <param name="flowChartTemplateShareDetailsDTO">The flowChartTemplateShareDetailsDTO.</param>
        /// <param name="ansid">The ansid.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="isUpdate">The isUpdate.</param>
        public async Task<bool> SaveSharetemplatetAsync(FlowChartTemplateShareDetailsDTO flowChartTemplateShareDetailsDTO, string ansid, Guid userId, CancellationToken cancellationToken, bool isUpdate = false)
        {
            // If you modifiy this method then please check the DefaultTemplateUpdater file also

            if (flowChartTemplateShareDetailsDTO.ComponentInfosList.Count > 0)
            {
                var template = await portal.FlowchartTemplates.GetByIdAsync(flowChartTemplateShareDetailsDTO.TemplateId, cancellationToken);

                template?.FlowchartDefinition.ThrowIfNull(nameof(template));
                var clientVersionDetais = await clientControllerLogic.GetAsync(flowChartTemplateShareDetailsDTO.CurrentOmniClientId, userId, cancellationToken);

                var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(flowChartTemplateShareDetailsDTO.CurrentOmniClientId, cancellationToken);

                DateTime calStartDate = DateTime.UtcNow;
                DateTime calEndDate = DateTime.UtcNow;

                FlowchartDefinition? flowchartDefinition = Json.Deserialize<FlowchartDefinition>(template?.FlowchartDefinition);

                FlowchartDefinition? newflowchartDefinition = new FlowchartDefinition();

                foreach (var component in flowChartTemplateShareDetailsDTO.ComponentInfosList)
                {
                    if (flowchartDefinition?.HeaderDefinition != null && component.TemplateId == flowchartDefinition.HeaderDefinition.TemplateId)
                    {
                        var headerTemplateDetails = await portalDbContext.HeaderTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.HeaderDefinition.TemplateId && x.Removed == false);
                        if (headerTemplateDetails != null)
                        {
                            var ht = new HeaderTemplate
                            {
                                Name = headerTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                HeaderDefinition = headerTemplateDetails.HeaderDefinition,
                                Version = 1,
                                Removed = false,
                            };
                            ht = await portal.HeaderTemplates.AddAsync(ht, cancellationToken);
                            newflowchartDefinition.HeaderDefinition = flowchartDefinition?.HeaderDefinition;
                            newflowchartDefinition.HeaderDefinition.TemplateId = ht.Id;
                            newflowchartDefinition.HeaderDefinition.TemplateVersion = ht.Version;
                        }
                    }
                    else if (flowchartDefinition?.CalendarDefinition != null && component.TemplateId == flowchartDefinition.CalendarDefinition.TemplateId)
                    {
                        var calendarTemplateDetails = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarDefinition.TemplateId && x.Removed == false);
                        if (calendarTemplateDetails != null)
                        {
                            var cd = new CalendarTemplate
                            {
                                Name = calendarTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                CalendarDefinition = calendarTemplateDetails.CalendarDefinition,
                                Version = 1,
                                Removed = false,
                            };
                            cd = await portal.CalendarTemplates.AddAsync(cd, cancellationToken);
                            newflowchartDefinition.CalendarDefinition = flowchartDefinition?.CalendarDefinition;
                            newflowchartDefinition.CalendarDefinition.TemplateId = cd.Id;
                            newflowchartDefinition.CalendarDefinition.TemplateVersion = cd.Version;
                        }
                        var res = Json.Deserialize<CalendarDefinition>(calendarTemplateDetails.CalendarDefinition);
                        if (res.Configuration.IsReportingTimeFrame && res.Configuration.ReportingTimeFrame == CalendarReportingTimeFrame.CurrentYear)
                        {
                            calStartDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                            calEndDate = new DateTime(DateTime.UtcNow.Year, 12, 31);
                        }
                        else if (res.Configuration.IsReportingTimeFrame && res.Configuration.ReportingTimeFrame == CalendarReportingTimeFrame.LastYear)
                        {
                            calStartDate = new DateTime(DateTime.UtcNow.Year - 1, 1, 1);
                            calEndDate = new DateTime(DateTime.UtcNow.Year - 1, 12, 31);
                        }
                        else
                        {
                            calStartDate = (DateTime)res.Configuration.CustomStartDate;
                            calEndDate = (DateTime)res.Configuration.CustomEndDate;
                        }
                    }
                    else if (flowchartDefinition?.CalendarOverlayDefinition != null && component.TemplateId == flowchartDefinition.CalendarOverlayDefinition.TemplateId)
                    {
                        var calendarOverlayTemplateDetails = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarOverlayDefinition.TemplateId && x.Removed == false);
                        if (calendarOverlayTemplateDetails != null)
                        {
                            var co = new CalendarOverlayTemplate
                            {
                                Name = calendarOverlayTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                CalendarOverlayDefinition = calendarOverlayTemplateDetails.CalendarOverlayDefinition,
                                Version = 1,
                                Removed = false,
                            };
                            co = await portal.CalendarOverlayTemplates.AddAsync(co, cancellationToken);
                            newflowchartDefinition.CalendarOverlayDefinition = flowchartDefinition?.CalendarOverlayDefinition;
                            newflowchartDefinition.CalendarOverlayDefinition.TemplateId = co.Id;
                            newflowchartDefinition.CalendarOverlayDefinition.TemplateVersion = co.Version;
                        }
                    }
                    else if (flowchartDefinition?.MediaHierarchyDefinition != null && flowchartDefinition.MediaHierarchyDefinition.Definition.Levels != null && component.TemplateId == flowchartDefinition.MediaHierarchyDefinition.TemplateId)
                    {
                        var mediaHierarchyTemplateDetails = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.MediaHierarchyDefinition.TemplateId && x.Removed == false);
                        for (int i = 0; i < flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.Count; i++)
                        {
                            var levelData = await GetPreviousLevelsData(flowchartDefinition.MediaHierarchyDefinition.Definition.Levels, i);

                            var getDistinctDataDto = new DistinctDataDTO()
                            {
                                ParentColumnName = null,
                                ParentSelectedValue = null,
                                ParentTableId = null,
                                Levels = levelData,
                                OmniClientId = (Guid)(flowChartTemplateShareDetailsDTO?.NewOmniClientId),
                                TableId = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].TableId,
                                ColumnName = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].ColumnName,
                                StartDate = calStartDate,
                                EndDate = calEndDate,
                            };

                            var dataMH = omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansid, mediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken) : await controllerLogic.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, mediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken);

                            Guid metricTableId = omniClientInfoDTO.Version > 1 ? DefaultMetricTableIdPMDS : DefaultMetricTableId;

                            bool matchExists = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList().Any(x => dataMH.ToList().Contains(x.Name));
                            var matched = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.Where(x => dataMH.ToList().Contains(x.Name)).ToList();

                            var isAnySettingSelected = matched.Any(c => c.Enabled);
                            foreach (var setting in matched.Select((value, index) => new { value, index }))
                            {
                                if (!isAnySettingSelected)
                                {
                                    setting.value.Enabled = true;
                                }
                                setting.value.Order = setting.index;
                            }

                            flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings = !matchExists ? dataMH.OrderBy(x => x).Select((x, index) => new MediaHierarchySetting()
                            {
                                Name = x.ToString(),
                                FlightRange = FlightRange.FlightTotal,
                                InflightOverlayStyling = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[0].Styling,
                                Enabled = true,
                                MetricTableId = metricTableId,
                                MetricColumnName = DefaultMetricColumnName,
                                Order = index,
                                Styling = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[0].Styling
                            }).ToList() : matched;

                            for (int j = 0; j < flowchartDefinition.MediaHierarchyDefinition.Definition.Levels?.ToList()[i].Settings.ToList().Count; j++)
                            {
                                if (flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels?.ToList()?.Count() > 0)
                                {
                                    for (int k = 0; k < flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings?.ToList()[j].SubLevels?.ToList().Count; k++)
                                    {
                                        var sublevelData = await GetPreviousLevelsData(flowchartDefinition.MediaHierarchyDefinition.Definition.Levels, i, flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels, k);

                                        if (sublevelData.Count > 0)
                                        {
                                            var currentSubLevel = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k];
                                            var getSubLevelDistinctDataDto = new DistinctDataDTO()
                                            {
                                                ParentColumnName = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].ColumnName,
                                                ParentSelectedValue = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings?.ToList()[j].Name,
                                                ParentTableId = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].TableId,
                                                Levels = sublevelData,
                                                OmniClientId = (Guid)(flowChartTemplateShareDetailsDTO?.NewOmniClientId),
                                                TableId = currentSubLevel.TableId,
                                                ColumnName = currentSubLevel.ColumnName,
                                                StartDate = calStartDate,
                                                EndDate = calEndDate,
                                            };

                                            var sublevelDataMH = omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(getSubLevelDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansid, mediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken) : await controllerLogic.GetDistinctDataAsync(getSubLevelDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, mediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken);

                                            Guid subLevelMetricTableId = omniClientInfoDTO.Version > 1 ? DefaultMetricTableIdPMDS : DefaultMetricTableId;

                                            bool subLevelMatchExists = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings.ToList().Any(x => sublevelDataMH.ToList().Contains(x.Name));

                                            var subLevelMatched = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings.Where(x => sublevelDataMH.ToList().Contains(x.Name)).ToList();

                                            var isAnySettingSelectedAtSublevel = subLevelMatched.Any(c => c.Enabled);

                                            foreach (var setting in subLevelMatched.Select((value, index) => new { value, index }))
                                            {
                                                if (!isAnySettingSelectedAtSublevel)
                                                {
                                                    setting.value.Enabled = true;
                                                }
                                                setting.value.Order = setting.index;
                                            }

                                            flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings = !subLevelMatchExists ? sublevelDataMH.OrderBy(x => x).Select((x, index) => new MediaHierarchySubLevelSetting()
                                            {
                                                Name = x.ToString(),
                                                FlightRange = FlightRange.FlightTotal,
                                                InflightOverlayStyling = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[0].Styling,
                                                Enabled = true,
                                                MetricTableId = subLevelMetricTableId,
                                                MetricColumnName = DefaultMetricColumnName,
                                                Order = index,
                                                Styling = flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings.ToList()[0].Styling
                                            }).ToList() : subLevelMatched;

                                        }
                                    }
                                }
                            }
                        }

                        if (mediaHierarchyTemplateDetails != null)
                        {
                            var mh = new MediaHierarchyTemplate
                            {
                                Name = mediaHierarchyTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                MediaHierarchyDefinition = Json.Serialize(flowchartDefinition?.MediaHierarchyDefinition.Definition),
                                Currency = mediaHierarchyTemplateDetails.Currency,
                                DisplaySource = mediaHierarchyTemplateDetails.DisplaySource,
                                Version = 1,
                                ShowSubTotalsAtBottom = mediaHierarchyTemplateDetails.ShowSubTotalsAtBottom,
                                Removed = false,
                            };
                            mh = await portal.MediaHierarchyTemplates.AddAsync(mh, cancellationToken);
                            newflowchartDefinition.MediaHierarchyDefinition = flowchartDefinition?.MediaHierarchyDefinition;
                            newflowchartDefinition.MediaHierarchyDefinition.TemplateId = mh.Id;
                            newflowchartDefinition.MediaHierarchyDefinition.TemplateVersion = mh.Version;
                        }
                    }
                    else if (flowchartDefinition?.TotalsDefinition != null && component.TemplateId == flowchartDefinition.TotalsDefinition.TemplateId)
                    {
                        var rightHandTotalsTemplateDetails = await portalDbContext.TotalsTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.TotalsDefinition.TemplateId && x.Removed == false);
                        if (rightHandTotalsTemplateDetails != null)
                        {
                            var rht = new TotalsTemplate
                            {
                                Name = rightHandTotalsTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                TotalsDefinition = rightHandTotalsTemplateDetails.TotalsDefinition,
                                Version = 1,
                                Removed = false,
                            };
                            rht = await portal.TotalsTemplates.AddAsync(rht, cancellationToken);
                            newflowchartDefinition.TotalsDefinition = flowchartDefinition?.TotalsDefinition;
                            newflowchartDefinition.TotalsDefinition.TemplateId = rht.Id;
                            newflowchartDefinition.TotalsDefinition.TemplateVersion = rht.Version;
                        }
                    }
                    else if (flowchartDefinition?.GrandTotalDefinition != null && component.TemplateId == flowchartDefinition.GrandTotalDefinition.TemplateId)
                    {
                        var grandTotalTemplateDetails = await portalDbContext.GrandTotalTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.GrandTotalDefinition.TemplateId && x.Removed == false);
                        if (grandTotalTemplateDetails != null)
                        {
                            var gt = new GrandTotalTemplate
                            {
                                Name = grandTotalTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                GrandTotalDefinition = grandTotalTemplateDetails.GrandTotalDefinition,
                                Version = 1,
                                Removed = false,
                            };
                            gt = await portal.GrandTotalTemplates.AddAsync(gt, cancellationToken);
                            newflowchartDefinition.GrandTotalDefinition = flowchartDefinition?.GrandTotalDefinition;
                            newflowchartDefinition.GrandTotalDefinition.TemplateId = gt.Id;
                            newflowchartDefinition.GrandTotalDefinition.TemplateVersion = gt.Version;
                        }
                    }
                    else if (flowchartDefinition?.ThemeDefinition != null && component.TemplateId == flowchartDefinition.ThemeDefinition.TemplateId)
                    {
                        var themeTemplateDetails = await portalDbContext.ThemeTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.ThemeDefinition.TemplateId && x.Removed == false);
                        if (themeTemplateDetails != null)
                        {
                            flowchartDefinition.ThemeDefinition.Definition = Json.Deserialize<ThemeDefinition>(themeTemplateDetails.ThemeDefinition);
                            if (flowchartDefinition.ThemeDefinition.Definition.LegendTheme != null && flowchartDefinition.ThemeDefinition.Definition.LegendTheme.ColumnName != null && flowchartDefinition.ThemeDefinition.Definition.LegendTheme.TableId != null)
                            {
                                var getDistinctDataDto = new DistinctDataDTO()
                                {
                                    ParentColumnName = null,
                                    ParentSelectedValue = null,
                                    ParentTableId = null,
                                    OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                    TableId = (Guid)flowchartDefinition.ThemeDefinition.Definition.LegendTheme.TableId,
                                    ColumnName = flowchartDefinition.ThemeDefinition.Definition.LegendTheme.ColumnName,
                                    StartDate = calStartDate,
                                    EndDate = calEndDate,
                                };

                                var dataMH = omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansid, themeTemplateDetails.CreatedByUserId, cancellationToken) : await controllerLogic.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, themeTemplateDetails.CreatedByUserId, cancellationToken);

                                bool matchExists = flowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.ToList().Any(x => dataMH.ToList().Contains(x.Name));
                                var matched = flowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.Where(x => dataMH.ToList().Contains(x.Name)).ToList();

                                var isAnySettingSelected = matched.Any(c => c.Enabled);
                                foreach (var setting in matched.Select((value, index) => new { value, index }))
                                {
                                    if (!isAnySettingSelected)
                                    {
                                        setting.value.Enabled = true;
                                    }
                                }

                                flowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings = !matchExists ? dataMH.OrderBy(x => x).Select((x, index) => new LegendThemeSetting()
                                {
                                    Name = x.ToString(),
                                    InflightOverlayStyling = flowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.ToList()[0].Styling,
                                    Enabled = true,
                                    Styling = flowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.ToList()[0].Styling
                                }).ToList() : matched;
                            }

                            var tt = new ThemeTemplate
                            {
                                Name = themeTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                ThemeDefinition = Json.Serialize(flowchartDefinition.ThemeDefinition.Definition),
                                Version = 1,
                                Removed = false,
                            };
                            tt = await portal.ThemeTemplates.AddAsync(tt, cancellationToken);
                            newflowchartDefinition.ThemeDefinition = flowchartDefinition?.ThemeDefinition;
                            newflowchartDefinition.ThemeDefinition.TemplateId = tt.Id;
                            newflowchartDefinition.ThemeDefinition.TemplateVersion = tt.Version;
                        }
                    }
                    else if (flowchartDefinition?.FooterDefinition != null && component.TemplateId == flowchartDefinition.FooterDefinition.TemplateId)
                    {
                        var footerTemplateDetails = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.FooterDefinition.TemplateId && x.Removed == false);
                        if (footerTemplateDetails != null)
                        {
                            var ft = new FooterTemplate
                            {
                                Name = footerTemplateDetails.Name,
                                CreatedByUserId = userId,
                                ModifiedByUserId = userId,
                                OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                                FooterDefinition = footerTemplateDetails.FooterDefinition,
                                Version = 1,
                                Removed = false,
                            };
                            ft = await portal.FooterTemplates.AddAsync(ft, cancellationToken);
                            newflowchartDefinition.FooterDefinition = flowchartDefinition?.FooterDefinition;
                            newflowchartDefinition.FooterDefinition.TemplateId = ft.Id;
                            newflowchartDefinition.FooterDefinition.TemplateVersion = ft.Version;
                        }
                    }
                }

                var newTemplate = new FlowchartTemplate
                {
                    Name = flowChartTemplateShareDetailsDTO.NewTemplateName,
                    CreatedByUserId = userId,
                    ModifiedByUserId = userId,
                    OmniClientId = flowChartTemplateShareDetailsDTO.NewOmniClientId,
                    FlowchartDefinition = Json.Serialize(newflowchartDefinition),
                    Version = 1,
                    Removed = false,
                    ParentId = flowChartTemplateShareDetailsDTO.TemplateId,
                };

                newTemplate = await portal.FlowchartTemplates.AddAsync(newTemplate, cancellationToken);

                log.Add(LogLevel.Information, $"Creating a flowchart template name: {newTemplate.Name} for the OmniClientId {newTemplate.OmniClientId} by user: {newTemplate.CreatedByUserId}");

                await portal.SaveChangesAsync(cancellationToken);

                return newTemplate != null ? true : false;
            }
            else
            {
                bool isSaved = await SaveDefaultFlowChartAsync(flowChartTemplateShareDetailsDTO.NewTemplateName, flowChartTemplateShareDetailsDTO.NewOmniClientId, flowChartTemplateShareDetailsDTO.CurrentOmniClientId, ansid, cancellationToken, isUpdate);
                return isSaved ? true : false;
            }
        }

        /// <summary>
        /// Save As flowchart template asynchronous.
        /// </summary>
        /// <param name="saveFlowchartDTO">The saveFlowchartDTO.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ReportInfoDTO> SaveAsAsync(SaveFlowchartDTO saveFlowchartDTO, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(saveFlowchartDTO.OmniClientId, user.Id, cancellationToken);

            if (await portal.FlowchartTemplates.AnyAsync(cancellationToken, x => x.Name == saveFlowchartDTO.TemplateName && x.OmniClientId == saveFlowchartDTO.OmniClientId))
            {
                throw new ArgumentException($"Template already exists with this name.");
            }

            if (saveFlowchartDTO.ReportPublish == null)
            {
                throw new ArgumentNullException(nameof(saveFlowchartDTO.ReportPublish));
            }

            #region // Start - Saving data in FlowchartTemplate table

            var flowchartTemplate = new FlowchartTemplate
            {
                Name = saveFlowchartDTO.TemplateName,
                OmniClientId = saveFlowchartDTO.OmniClientId,
                Version = saveFlowchartDTO.Version ?? 1,
                FlowchartDefinition = saveFlowchartDTO.Definition == null ? null : Json.Serialize(saveFlowchartDTO.Definition),
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                IsDefault = saveFlowchartDTO.IsDefault,
                Removed = false,
            };

            flowchartTemplate = await portal.FlowchartTemplates.AddAsync(flowchartTemplate, cancellationToken);
            log.Add(LogLevel.Information, $"Creating a flowchart template: {flowchartTemplate.Id} by user: {flowchartTemplate.CreatedByUserId}");

            #endregion // End - Saving data in FlowchartTemplate table

            #region // Start - Saving data in FlowchartTemplatesVersionHistorty table

            var editVersionHistory = new FlowchartTemplatesVersionHistorty
            {
                FlowchartTemplateId = flowchartTemplate.Id,
                Name = saveFlowchartDTO.TemplateName,
                FlowchartDefinition = saveFlowchartDTO.Definition == null ? null : Json.Serialize(saveFlowchartDTO.Definition),
                Comments = saveFlowchartDTO.Comments ?? "Initial Version",
                Version = saveFlowchartDTO.Version ?? 1,
                CustomStyleSettings = saveFlowchartDTO.CustomStyleSettings == null ? null : saveFlowchartDTO.CustomStyleSettings,
                TrackFormatChanges = saveFlowchartDTO.TrackFormatChanges == null ? null : saveFlowchartDTO.TrackFormatChanges,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };

            editVersionHistory = await portal.FlowchartTemplatesVersionHistorties.AddAsync(editVersionHistory, cancellationToken);
            log.Add(LogLevel.Information, $"Creating a flowchart Template Version Historties: {editVersionHistory.Id} for template {flowchartTemplate.Id} by user: {user.Id}");

            #endregion // End - Saving data in FlowchartTemplatesVersionHistorty table

            #region // Start - Saving data in RunConfiguration table

            var runConfiguration = new RunConfiguration
            {
                FlowchartTemplateId = flowchartTemplate.Id,
                Name = saveFlowchartDTO.TemplateName,
                CalendarDefinition = saveFlowchartDTO.Definition == null ? null : Json.Serialize(saveFlowchartDTO.CalendarDefinition),
                MediaHierarchyLevels = saveFlowchartDTO.MediaHierarchyLevels == null ? null : Json.Serialize(saveFlowchartDTO.MediaHierarchyLevels ?? []),
                RunRestrictions = saveFlowchartDTO.RunRestrictions == null ? null : Json.Serialize(saveFlowchartDTO.RunRestrictions ?? []),
                Comments = saveFlowchartDTO.Comments ?? "Initial Version",
                Version = saveFlowchartDTO.Version ?? 1,
                CustomStyleSettings = saveFlowchartDTO.CustomStyleSettings == null ? null : saveFlowchartDTO.CustomStyleSettings,
                TrackFormatChanges = saveFlowchartDTO.TrackFormatChanges == null ? null : saveFlowchartDTO.TrackFormatChanges,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };

            runConfiguration = await portal.RunConfigurations.AddAsync(runConfiguration, cancellationToken);
            log.Add(LogLevel.Information, $"Creating run configuration: {runConfiguration.Id} for template {flowchartTemplate.Id} by user: {user.Id}");

            #endregion // End - Saving data in RunConfiguration table

            #region // Start - Saving data in Report table

            var report = new Report
            {
                FlowchartTemplateId = flowchartTemplate.Id,
                RunConfigurationId = runConfiguration.Id,
                Name = saveFlowchartDTO.ReportPublish.Name,
                FileType = saveFlowchartDTO.ReportPublish.FileType,
                Comments = saveFlowchartDTO.ReportPublish.Comments ?? "Initial Version",
                FlowchartTemplateVersion = saveFlowchartDTO.Version ?? 1,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };

            report = await portal.Reports.AddAsync(report, cancellationToken);
            log.Add(LogLevel.Information, $"Creating a report: {flowchartTemplate.Id} for template {flowchartTemplate.Id} by user: {user.Id}");

            using var file = new MemoryStream(saveFlowchartDTO.ReportPublish.FileContent);
            using var stream = await CleanupExcelDocumentAsync(file, cancellationToken);
            await storage.UploadAsync(stream, $"{report.Id}{ExcelFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            using var dataStream = new MemoryStream();
            Json.Serialize(saveFlowchartDTO.ReportPublish.Data, dataStream);
            dataStream.Position = 0;
            await storage.UploadAsync(dataStream, $"{report.Id}{DataFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);
            using var templateStream = new MemoryStream();
            Json.Serialize(runConfiguration.FlowchartTemplate, templateStream);
            templateStream.Position = 0;
            await storage.UploadAsync(templateStream, $"{report.Id}{FlowchartTemplateFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            using var settingsStream = new MemoryStream();
            Json.Serialize(saveFlowchartDTO.ReportPublish.RunConfigurationData, settingsStream);
            settingsStream.Position = 0;
            await storage.UploadAsync(settingsStream, $"{report.Id}{RunConfigFileExtension}", storageConfig.CurrentValue.ContainerName, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<ReportInfoDTO>(report);

            result.CreatedByUser = result.ModifiedByUser = user;
            result.RunConfiguration = mapper.Map<RunConfigurationInfoDTO>(runConfiguration);

            return result;
            #endregion // End - Saving data in Report table

        }

        /// <summary>
        /// Cleans up the excel document asynchronous.
        /// </summary>
        /// <param name="file">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<Stream> CleanupExcelDocumentAsync(Stream file, CancellationToken cancellationToken)
        {
            var output = new MemoryStream();
            await file.CopyToAsync(output, cancellationToken);
            output.Position = 0;

            using (var doc = SpreadsheetDocument.Open(output, true))
            {
                if (doc.WebExTaskpanesPart != null)
                {
                    doc.DeletePart(doc.WebExTaskpanesPart);
                }

                if (doc.CustomFilePropertiesPart != null)
                {
                    doc.DeletePart(doc.CustomFilePropertiesPart);
                }
            }

            output.Position = 0;

            return output;
        }

        /*        /// <summary>
                /// Checks if the template has been shared with other clients
                /// </summary>
                /// <param name="templateId">The template identifier.</param>
                /// <param name="userId">The user identifier.</param>
                /// <param name="cancellationToken">The cancellation token.</param>
                public async Task<bool> IsTemplateShared(Guid templateId, Guid userId, CancellationToken cancellationToken)
                {
                    var template = await portal.FlowchartTemplates.GetByIdAsync(templateId, cancellationToken);
                    await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

                    log.Add(LogLevel.Information, $"Checking if template {templateId} has been shared by user: {userId}");
                    return await portal.FlowchartTemplates.AnyAsync(cancellationToken, t => t.ParentId == templateId);
                }

                /// <summary>
                /// Checks if the template was received from another client
                /// </summary>
                /// <param name="templateId">The template identifier.</param>
                /// <param name="userId">The user identifier.</param>
                /// <param name="cancellationToken">The cancellation token.</param>
                public async Task<bool> IsTemplateReceived(Guid templateId, Guid userId, CancellationToken cancellationToken)
                {
                    var template = await portal.FlowchartTemplates.GetByIdAsync(templateId, cancellationToken);
                    await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

                    log.Add(LogLevel.Information, $"Checking if template {templateId} was received by user: {userId}");
                    return template.ParentId.HasValue;
                }*/

        /// <summary>
        /// Retrieves data for previous levels up to the specified level index.
        /// </summary>
        /// <param name="levels">The collection of media hierarchy levels.</param>
        /// <param name="levelIndex">The index of the level up to which data is to be retrieved.</param>
        /// <returns>A list of LevelDataDTO containing data for previous levels.</returns>
        public async Task<List<LevelDataDTO>> GetPreviousLevelsData(ICollection<MediaHierarchyLevel>? levels, int levelIndex, ICollection<MediaHierarchySubLevel>? sublevels = null, int? sublevelIndex = 0)
        {
            var previousLevelsData = new List<LevelDataDTO>();

            if (levels != null)
            {
                int index = 0;
                foreach (var l in levels)
                {
                    if (index < levelIndex)
                    {
                        var selectedValues = l.Settings.Where(s => s.Enabled).Select(c => c.Name).ToList();
                        if (selectedValues.Count > 0)
                        {
                            previousLevelsData.Add(new LevelDataDTO { ColumnName = l.ColumnName, TableId = l.TableId, SelectedValues = selectedValues.ToArray() });
                        }
                    }
                    index++;
                }
            }

            if (sublevels != null)
            {
                int index = 0;
                foreach (var l in sublevels)
                {
                    if (index < sublevelIndex)
                    {
                        var selectedValues = l.Settings.Where(s => s.Enabled).Select(c => c.Name).ToList();
                        if (selectedValues.Count > 0)
                        {
                            previousLevelsData.Add(new LevelDataDTO { ColumnName = l.ColumnName, TableId = l.TableId, SelectedValues = selectedValues.ToArray() });
                        }
                    }
                    index++;
                }
            }
            return await Task.FromResult(previousLevelsData);
        }

        /// <summary>
        /// Update default template by flowchart template name asynchronous.
        /// </summary>
        /// <param name="defaultTemplates">The userId.</param>
        /// <param name="ansid">The ansid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="isUpdate">The isUpdate.</param>
        public async Task<bool> UpdateDefaultTemplateAsync(FlowchartTemplate defaultTemplates, string ansid, CancellationToken cancellationToken, Guid? omniClientId, bool isUpdate = false)
        {
            bool isUpdated = false;
            if (!isUpdate)
            {
                log.Add(LogLevel.Information, $"Updating default templates {defaultTemplates}.");

                var allOmniClients = (omniClientId.HasValue && omniClientId != Guid.Empty) ? await portalDbContext.OmniClients.Where(x => x.Id != defaultTemplates.OmniClientId && x.Id == omniClientId && !x.Removed).AsNoTracking().ToListAsync() : await portalDbContext.OmniClients.Where(x => x.Id != defaultTemplates.OmniClientId && !x.Removed).AsNoTracking().ToListAsync();

                var allFlowchartTemplates = await portalDbContext.FlowchartTemplates.AsNoTracking().Where(x => !x.Removed).AsNoTracking().ToListAsync();

                foreach (var omnclient in allOmniClients)
                {
                    bool exists = allFlowchartTemplates.Exists(x => x.Name == defaultTemplates.Name && x.OmniClientId == omnclient.Id && !x.Removed);

                    if (!exists)
                    {
                        var flowChartTemplateShareDetailsDTO = new FlowChartTemplateShareDetailsDTO
                        {
                            NewOmniClientId = omnclient.Id,
                            CurrentOmniClientId = defaultTemplates.OmniClientId,
                            NewTemplateName = defaultTemplates.Name,
                            TemplateId = defaultTemplates.Id,
                            ComponentInfosList = new List<ComponentInfo>(),
                        };
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        log.Add(LogLevel.Information, $"Update default template starts for template name: {defaultTemplates.Name} for clientId : {omnclient.Id}");
                        var backGroundJobId = BackgroundJob.Enqueue(() => SaveSharetemplatetAsync(flowChartTemplateShareDetailsDTO, ansid, defaultTemplates.CreatedByUserId, cancellationToken, isUpdate));
                        sw.Stop();
                        log.Add(LogLevel.Information, $"Update default template finished for template name: {defaultTemplates.Name} for clientId : {omnclient.Id} in {sw.Elapsed.TotalMinutes}");
                    }
                }
            }
            else
            {
                log.Add(LogLevel.Information, $"Updating default templates {defaultTemplates}.");

                var allOmniClientsWithDefaultTemplates = await portalDbContext.FlowchartTemplates.Where(x => x.Name == defaultTemplates.Name && x.OmniClientId != defaultTemplates.OmniClientId && !x.Removed).Select(t => new { t.Id, t.OmniClientId, t.Name }).AsNoTracking().Distinct().ToListAsync();
                foreach (var template in allOmniClientsWithDefaultTemplates)
                {
                    var flowChartTemplateShareDetailsDTO = new FlowChartTemplateShareDetailsDTO
                    {
                        NewOmniClientId = template.OmniClientId,
                        CurrentOmniClientId = defaultTemplates.OmniClientId,
                        NewTemplateName = template.Name,
                        TemplateId = template.Id,
                        ComponentInfosList = new List<ComponentInfo>(),
                    };
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    log.Add(LogLevel.Information, $"Update default template starts for template name: {defaultTemplates.Name} for clientId : {template.OmniClientId}");
                    var backGroundJobId = BackgroundJob.Enqueue(() => SaveSharetemplatetAsync(flowChartTemplateShareDetailsDTO, ansid, defaultTemplates.CreatedByUserId, cancellationToken, isUpdate));
                    sw.Stop();
                    log.Add(LogLevel.Information, $"Update default template finished for template name: {defaultTemplates.Name} for clientId : {template.OmniClientId} in {sw.Elapsed.TotalMinutes}");
                }


            }
            return isUpdated;
        }

        /// <summary>
        /// Checks if the template was received from another client
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="omniClientId">The omniClientId.</param>
        /// <param name="defaultOmniClientId">The defaultOmniClientId</param>
        /// <param name="ansid">The ansid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="isUpdate">The isUpdate.</param>
        public async Task<bool> SaveDefaultFlowChartAsync(string name, Guid omniClientId, Guid defaultOmniClientId, string ansid, CancellationToken cancellationToken, bool isUpdate = false)
        {
            try
            {
                var mappingDetails = await portalDbContext.PlannedMediaMigrationColumnMappings.Where(x => x.Removed == false).ToListAsync();
                var duplicates = mappingDetails.GroupBy(i => i.SourceColumnName).Where(x => x.Count() > 1).Select(val => val.Key).ToList();

                var defaultClientId = portalDbContext.OmniClients.FirstOrDefault(x => x.Id == defaultOmniClientId).ClientId;
                var defaultClientMappingDetails = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == defaultClientId, cancellationToken);

                var clientId = portalDbContext.OmniClients.FirstOrDefault(x => x.Id == omniClientId).ClientId;
                var clienctMappingDetails = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);

                bool isClientHasSameVersion = defaultClientMappingDetails.Version == clienctMappingDetails.Version ? true : false;

                if (!isClientHasSameVersion)
                {
                    defaultClientMappingDetails.Version = defaultClientMappingDetails.Version > 1 && defaultClientMappingDetails.Version > clienctMappingDetails.Version ? 2 : 1;
                }

                var defaultUserId = await GetDefaultTemplateUser(cancellationToken);
                bool result = false;
                if (!isUpdate)
                {
                    var template = await portalDbContext.FlowchartTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name && x.OmniClientId == defaultOmniClientId && !x.Removed);
                    template?.FlowchartDefinition.ThrowIfNull(nameof(template));

                    var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(omniClientId, cancellationToken);

                    DateTime calStartDate = DateTime.UtcNow;
                    DateTime calEndDate = DateTime.UtcNow;

                    if (defaultUserId != Guid.Empty)
                    {
                        template.CreatedByUserId = defaultUserId;
                        template.ModifiedByUserId = defaultUserId;
                    }

                    template = !isClientHasSameVersion ? await GetMigratedDefinitionOfFlowchartForPMDS(template, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : template;

                    FlowchartDefinition? flowchartDefinition = Json.Deserialize<FlowchartDefinition>(template?.FlowchartDefinition);

                    FlowchartDefinition? newflowchartDefinition = new FlowchartDefinition();

                    if (flowchartDefinition?.HeaderDefinition != null)
                    {
                        var headerTemplateDetails = await portalDbContext.HeaderTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.HeaderDefinition.TemplateId && x.Removed == false);

                        headerTemplateDetails = !isClientHasSameVersion && headerTemplateDetails != null ? await GetMigratedDefinitionOfHeaderForPMDS(headerTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : headerTemplateDetails;

                        var currentTemplate = await portalDbContext.HeaderTemplates.FirstOrDefaultAsync(x => headerTemplateDetails != null && x.Name == headerTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            var isSameVersion = headerTemplateDetails.Version > currentTemplate.Version;
                            if (isSameVersion)
                            {
                                if (headerTemplateDetails != null && currentTemplate != null)
                                {
                                    UpdateTemplateDetails(headerTemplateDetails, currentTemplate);
                                }
                            }
                            UpdateTemplate<HeaderTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else if (headerTemplateDetails != null)
                        {
                            await CreateDefaultTemplateHeader(flowchartDefinition, newflowchartDefinition, headerTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    if (flowchartDefinition?.CalendarDefinition != null)
                    {
                        var calendarTemplateDetails = await portalDbContext.CalendarTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarDefinition.TemplateId && x.Removed == false);

                        var currentTemplate = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => calendarTemplateDetails != null && x.Name == calendarTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);
                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            var isSameVersion = calendarTemplateDetails.Version > currentTemplate.Version;
                            if (isSameVersion)
                            {
                                if (calendarTemplateDetails != null && currentTemplate != null)
                                {
                                    UpdateTemplateDetails(calendarTemplateDetails, currentTemplate);
                                }
                            }
                            UpdateTemplate<CalendarTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else if (calendarTemplateDetails != null)
                        {
                            await CreateDefaultTemplateCalendar(flowchartDefinition, newflowchartDefinition, calendarTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                        var res = Json.Deserialize<CalendarDefinition>(calendarTemplateDetails.CalendarDefinition);
                        if (res.Configuration.IsReportingTimeFrame && res.Configuration.ReportingTimeFrame == CalendarReportingTimeFrame.CurrentYear)
                        {
                            calStartDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                            calEndDate = new DateTime(DateTime.UtcNow.Year, 12, 31);
                        }
                        else if (res.Configuration.IsReportingTimeFrame && res.Configuration.ReportingTimeFrame == CalendarReportingTimeFrame.LastYear)
                        {
                            calStartDate = new DateTime(DateTime.UtcNow.Year - 1, 1, 1);
                            calEndDate = new DateTime(DateTime.UtcNow.Year - 1, 12, 31);
                        }
                        else
                        {
                            calStartDate = (DateTime)res.Configuration.CustomStartDate;
                            calEndDate = (DateTime)res.Configuration.CustomEndDate;
                        }
                    }
                    if (flowchartDefinition?.CalendarOverlayDefinition != null)
                    {
                        var calendarOverlayTemplateDetails = await portalDbContext.CalendarOverlayTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarOverlayDefinition.TemplateId && x.Removed == false);

                        var currentTemplate = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => calendarOverlayTemplateDetails != null && x.Name == calendarOverlayTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);
                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            var isSameVersion = calendarOverlayTemplateDetails.Version > currentTemplate.Version;
                            if (isSameVersion)
                            {
                                if (calendarOverlayTemplateDetails != null && currentTemplate != null)
                                {
                                    UpdateTemplateDetails(calendarOverlayTemplateDetails, currentTemplate);
                                }
                            }
                            UpdateTemplate<CalendarOverlayTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else if (calendarOverlayTemplateDetails != null)
                        {
                            await CreateDefaultTemplateCalendarOverlay(flowchartDefinition, newflowchartDefinition, calendarOverlayTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    if (flowchartDefinition?.MediaHierarchyDefinition != null && flowchartDefinition.MediaHierarchyDefinition.Definition.Levels != null)
                    {
                        var mediaHierarchyTemplateDetails = await portalDbContext.MediaHierarchyTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.MediaHierarchyDefinition.TemplateId && x.Removed == false);

                        mediaHierarchyTemplateDetails = !isClientHasSameVersion && mediaHierarchyTemplateDetails != null ? await GetMigratedDefinitionOfMediaHierarchyForPMDS(mediaHierarchyTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : mediaHierarchyTemplateDetails;

                        var currentTemplate = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => mediaHierarchyTemplateDetails != null && x.Name == mediaHierarchyTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            bool isMediaHierarchySame = mediaHierarchyTemplateDetails.Version > currentTemplate.Version;
                            if (isMediaHierarchySame)
                            {
                                if (mediaHierarchyTemplateDetails != null)
                                {
                                    await CreateMediaHierarchyForDefaultTemplate(flowchartDefinition, mediaHierarchyTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);
                                    if (currentTemplate != null)
                                    {
                                        UpdateTemplateDetails(mediaHierarchyTemplateDetails, currentTemplate, flowchartDefinition);
                                    }
                                }
                            }
                            else
                            {
                                flowchartDefinition.MediaHierarchyDefinition.Definition = Json.Deserialize<MediaHierarchyDefinition>(currentTemplate?.MediaHierarchyDefinition);
                            }
                            UpdateTemplate<MediaHierarchyTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else if (mediaHierarchyTemplateDetails != null)
                        {
                            await CreateMediaHierarchyForDefaultTemplate(flowchartDefinition, mediaHierarchyTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);

                            await CreateDefaultTemplateMediaHierarchy(flowchartDefinition, newflowchartDefinition, mediaHierarchyTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    if (flowchartDefinition?.TotalsDefinition != null)
                    {
                        var rightHandTotalsTemplateDetails = await portalDbContext.TotalsTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.TotalsDefinition.TemplateId && x.Removed == false);

                        rightHandTotalsTemplateDetails = !isClientHasSameVersion && rightHandTotalsTemplateDetails != null ? await GetMigratedDefinitionOfRightHandTotalForPMDS(rightHandTotalsTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : rightHandTotalsTemplateDetails;

                        var currentTemplate = await portalDbContext.TotalsTemplates.FirstOrDefaultAsync(x => rightHandTotalsTemplateDetails != null && x.Name == rightHandTotalsTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);
                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            var isSameVersion = rightHandTotalsTemplateDetails.Version > currentTemplate.Version;
                            if (isSameVersion)
                            {
                                if (rightHandTotalsTemplateDetails != null && currentTemplate != null)
                                {
                                    UpdateTemplateDetails(rightHandTotalsTemplateDetails, currentTemplate);
                                }
                            }
                            UpdateTemplate<TotalsTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else if (rightHandTotalsTemplateDetails != null)
                        {
                            await CreateDefaultTemplateRightHandTotal(flowchartDefinition, newflowchartDefinition, rightHandTotalsTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    if (flowchartDefinition?.GrandTotalDefinition != null)
                    {
                        var grandTotalTemplateDetails = await portalDbContext.GrandTotalTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.GrandTotalDefinition.TemplateId && x.Removed == false);

                        grandTotalTemplateDetails = !isClientHasSameVersion && grandTotalTemplateDetails != null ? await GetMigratedDefinitionOfGrandTotalForPMDS(grandTotalTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : grandTotalTemplateDetails;

                        var currentTemplate = await portalDbContext.GrandTotalTemplates.FirstOrDefaultAsync(x => grandTotalTemplateDetails != null && x.Name == grandTotalTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);
                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            var isSameVersion = grandTotalTemplateDetails.Version > currentTemplate.Version;
                            if (isSameVersion)
                            {
                                if (grandTotalTemplateDetails != null && currentTemplate != null)
                                {
                                    UpdateTemplateDetails(grandTotalTemplateDetails, currentTemplate);
                                }
                            }
                            UpdateTemplate<GrandTotalTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else
                        if (grandTotalTemplateDetails != null)
                        {
                            await CreateDefaultTemplateGrandTotal(flowchartDefinition, newflowchartDefinition, grandTotalTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    if (flowchartDefinition?.ThemeDefinition != null)
                    {
                        var themeTemplateDetails = await portalDbContext.ThemeTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.ThemeDefinition.TemplateId && x.Removed == false);

                        themeTemplateDetails = !isClientHasSameVersion && themeTemplateDetails != null ? await GetMigratedDefinitionOfThemeForPMDS(themeTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : themeTemplateDetails;

                        var currentTemplate = await portalDbContext.ThemeTemplates.FirstOrDefaultAsync(x => themeTemplateDetails != null && x.Name == themeTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            bool isThemeSame = themeTemplateDetails.Version > currentTemplate.Version;
                            if (isThemeSame)
                            {
                                if (themeTemplateDetails != null && currentTemplate != null)
                                {
                                    await CreateThemeForDefaultTemplate(flowchartDefinition, themeTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);

                                    UpdateTemplateDetails(themeTemplateDetails, currentTemplate, flowchartDefinition);
                                }
                            }
                            else
                            {
                                flowchartDefinition.ThemeDefinition.Definition = Json.Deserialize<ThemeDefinition>(currentTemplate?.ThemeDefinition);
                            }
                            UpdateTemplate<ThemeTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else if (themeTemplateDetails != null)
                        {
                            await CreateThemeForDefaultTemplate(flowchartDefinition, themeTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);

                            await CreateDefaultTemplateTheme(flowchartDefinition, newflowchartDefinition, themeTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    if (flowchartDefinition?.FooterDefinition != null)
                    {
                        var footerTemplateDetails = await portalDbContext.FooterTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == flowchartDefinition.FooterDefinition.TemplateId && x.Removed == false);
                        var currentTemplate = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => footerTemplateDetails != null && x.Name == footerTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);
                        if (currentTemplate != null && !string.IsNullOrWhiteSpace(currentTemplate?.Name))
                        {
                            var isSameVersion = footerTemplateDetails.Version > currentTemplate.Version;
                            if (isSameVersion)
                            {
                                if (footerTemplateDetails != null && currentTemplate != null)
                                {
                                    UpdateTemplateDetails(footerTemplateDetails, currentTemplate);
                                }
                            }
                            UpdateTemplate<FooterTemplate>(flowchartDefinition, newflowchartDefinition, currentTemplate);
                        }
                        else
                        if (footerTemplateDetails != null)
                        {
                            await CreateDefaultTemplateFooter(flowchartDefinition, newflowchartDefinition, footerTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }

                    // create only if all MH levels should have atleast one settings
                    // also check if each sub level settings should have atleast one settings

                    if (flowchartDefinition?.MediaHierarchyDefinition != null
                        && flowchartDefinition.MediaHierarchyDefinition?.Definition?.Levels != null
                        && flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.All(x => x.Settings.Any(s => s.Enabled))
                        && flowchartDefinition.MediaHierarchyDefinition.Definition.Levels.All(x =>
                        x.Settings.All(s => s.SubLevels == null || s.SubLevels.All(sub => sub.Settings.Any(subSetting => subSetting.Enabled))))
                        )
                    {
                        var newTemplate = new FlowchartTemplate
                        {
                            Name = template.Name,
                            CreatedByUserId = template.CreatedByUserId,
                            ModifiedByUserId = template.ModifiedByUserId,
                            OmniClientId = omniClientId,
                            FlowchartDefinition = Json.Serialize(newflowchartDefinition),
                            Version = template.Version,
                            Removed = false,
                            ParentId = template.Id,
                            IsDefault = template.IsDefault,
                            ModifiedDate = DateTime.UtcNow,
                        };

                        newTemplate = await portal.FlowchartTemplates.AddAsync(newTemplate, cancellationToken);

                        log.Add(LogLevel.Information, $"BackgroundJob: Creating a flowchart with template name: {newTemplate.Name} for the client: {omniClientInfoDTO.Client.Name} and OmniClientId: {newTemplate.OmniClientId} by user: {newTemplate.CreatedByUserId}");

                        await portal.SaveChangesAsync(cancellationToken);

                        log.Add(LogLevel.Information, $"BackgroundJob: Created a flowchart template name: {newTemplate.Name} for the client: {omniClientInfoDTO.Client.Name} and OmniClientId: {newTemplate.OmniClientId} by user: {newTemplate.CreatedByUserId}");

                        result = newTemplate != null;
                    }
                    else
                    {
                        log.Add(LogLevel.Information, $"BackgroundJob: Could not create a flowchart template with name: {template.Name} for the client: {omniClientInfoDTO.Client.Name} and OmniClientId: {omniClientId} as the data is not available for selected date range");

                        result = false;
                    }
                }
                else
                {
                    var defautTemplate = await portalDbContext.FlowchartTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name && x.OmniClientId == defaultOmniClientId && !x.Removed);
                    defautTemplate?.FlowchartDefinition.ThrowIfNull(nameof(defautTemplate));

                    defautTemplate = !isClientHasSameVersion ? await GetMigratedDefinitionOfFlowchartForPMDS(defautTemplate, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : defautTemplate;

                    var currentTemplate = await portalDbContext.FlowchartTemplates.FirstOrDefaultAsync(x => x.Name == name && x.OmniClientId == omniClientId && !x.Removed);
                    currentTemplate?.FlowchartDefinition.ThrowIfNull(nameof(currentTemplate));

                    var omniClientInfoDTO = await clientControllerLogic.GetAllOmniClientDetailsByOmniClientId(omniClientId, cancellationToken);

                    DateTime calStartDate = DateTime.UtcNow;
                    DateTime calEndDate = DateTime.UtcNow;

                    FlowchartDefinition? defaultFlowchartDefinition = Json.Deserialize<FlowchartDefinition>(defautTemplate?.FlowchartDefinition);

                    FlowchartDefinition? currentFlowchartDefinition = Json.Deserialize<FlowchartDefinition>(currentTemplate?.FlowchartDefinition);

                    if (defaultFlowchartDefinition?.HeaderDefinition != null)
                    {
                        var defaultHeaderTemplateDetails = await portalDbContext.HeaderTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.HeaderDefinition.TemplateId && x.Removed == false);

                        defaultHeaderTemplateDetails = !isClientHasSameVersion && defaultHeaderTemplateDetails != null ? await GetMigratedDefinitionOfHeaderForPMDS(defaultHeaderTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : defaultHeaderTemplateDetails;

                        var currentHeaderTemplateDetails = await portalDbContext.HeaderTemplates.FirstOrDefaultAsync(x => defaultHeaderTemplateDetails != null && x.Name == defaultHeaderTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentHeaderTemplateDetails != null && !string.IsNullOrWhiteSpace(currentHeaderTemplateDetails?.Name))
                        {
                            bool isHeaderSame = defaultHeaderTemplateDetails.Version > currentHeaderTemplateDetails.Version;
                            if (isHeaderSame)
                            {
                                if (defaultHeaderTemplateDetails != null && currentHeaderTemplateDetails != null)
                                {
                                    UpdateTemplateDetails(defaultHeaderTemplateDetails, currentHeaderTemplateDetails);
                                }
                            }
                            UpdateTemplate<HeaderTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentHeaderTemplateDetails);
                        }
                        else
                        {
                            await CreateDefaultTemplateHeader(defaultFlowchartDefinition, currentFlowchartDefinition, defaultHeaderTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.HeaderDefinition = defaultFlowchartDefinition?.HeaderDefinition;
                        if (currentFlowchartDefinition.HeaderDefinition != null)
                        {
                            currentFlowchartDefinition.HeaderDefinition.TemplateVersion = defaultFlowchartDefinition?.HeaderDefinition?.TemplateVersion;
                            currentFlowchartDefinition.HeaderDefinition.IsDefault = defaultFlowchartDefinition?.HeaderDefinition?.IsDefault;
                        }
                    }

                    if (defaultFlowchartDefinition?.CalendarDefinition != null)
                    {
                        var defaultCalendarTemplateDetails = await portalDbContext.CalendarTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.CalendarDefinition.TemplateId && x.Removed == false);
                        var currentCalendarTemplateDetails = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => defaultCalendarTemplateDetails != null && x.Name == defaultCalendarTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        var res = Json.Deserialize<CalendarDefinition>(defaultCalendarTemplateDetails.CalendarDefinition);
                        if (res.Configuration.IsReportingTimeFrame && res.Configuration.ReportingTimeFrame == CalendarReportingTimeFrame.CurrentYear)
                        {
                            calStartDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                            calEndDate = new DateTime(DateTime.UtcNow.Year, 12, 31);
                        }
                        else if (res.Configuration.IsReportingTimeFrame && res.Configuration.ReportingTimeFrame == CalendarReportingTimeFrame.LastYear)
                        {
                            calStartDate = new DateTime(DateTime.UtcNow.Year - 1, 1, 1);
                            calEndDate = new DateTime(DateTime.UtcNow.Year - 1, 12, 31);
                        }
                        else
                        {
                            calStartDate = (DateTime)res.Configuration.CustomStartDate;
                            calEndDate = (DateTime)res.Configuration.CustomEndDate;
                        }

                        if (currentCalendarTemplateDetails != null && !string.IsNullOrWhiteSpace(currentCalendarTemplateDetails?.Name))
                        {
                            bool isCalendarSame = defaultCalendarTemplateDetails.Version > currentCalendarTemplateDetails.Version;

                            if (isCalendarSame)
                            {
                                if (defaultCalendarTemplateDetails != null && currentCalendarTemplateDetails != null)
                                {
                                    UpdateTemplateDetails(defaultCalendarTemplateDetails, currentCalendarTemplateDetails);
                                }
                            }
                            UpdateTemplate<CalendarTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentCalendarTemplateDetails);
                        }
                        else
                        {
                            await CreateDefaultTemplateCalendar(defaultFlowchartDefinition, currentFlowchartDefinition, defaultCalendarTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.CalendarDefinition = defaultFlowchartDefinition?.CalendarDefinition;
                        if (currentFlowchartDefinition.CalendarDefinition != null)
                        {
                            currentFlowchartDefinition.CalendarDefinition.TemplateVersion = defaultFlowchartDefinition?.CalendarDefinition?.TemplateVersion;
                            currentFlowchartDefinition.CalendarDefinition.IsDefault = defaultFlowchartDefinition?.CalendarDefinition?.IsDefault;
                        }
                    }

                    if (defaultFlowchartDefinition?.CalendarOverlayDefinition != null)
                    {
                        var defaultCalendarOverlayTemplateDetails = await portalDbContext.CalendarOverlayTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.CalendarOverlayDefinition.TemplateId && x.Removed == false);
                        var currentCalendarOverlayTemplateDetails = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => defaultCalendarOverlayTemplateDetails != null && x.Name == defaultCalendarOverlayTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentCalendarOverlayTemplateDetails != null && !string.IsNullOrWhiteSpace(currentCalendarOverlayTemplateDetails?.Name))
                        {
                            bool isCalendarOverlaySame = defaultCalendarOverlayTemplateDetails.Version > currentCalendarOverlayTemplateDetails.Version;
                            if (isCalendarOverlaySame)
                            {
                                if (defaultCalendarOverlayTemplateDetails != null && currentCalendarOverlayTemplateDetails != null)
                                {
                                    UpdateTemplateDetails(defaultCalendarOverlayTemplateDetails, currentCalendarOverlayTemplateDetails);
                                }
                            }
                            UpdateTemplate<CalendarOverlayTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentCalendarOverlayTemplateDetails);
                        }
                        else
                        {
                            await CreateDefaultTemplateCalendarOverlay(defaultFlowchartDefinition, currentFlowchartDefinition, defaultCalendarOverlayTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.CalendarOverlayDefinition = defaultFlowchartDefinition?.CalendarOverlayDefinition;
                        if (currentFlowchartDefinition.CalendarOverlayDefinition != null)
                        {
                            currentFlowchartDefinition.CalendarOverlayDefinition.TemplateVersion = defaultFlowchartDefinition?.CalendarOverlayDefinition?.TemplateVersion;
                            currentFlowchartDefinition.CalendarOverlayDefinition.IsDefault = defaultFlowchartDefinition?.CalendarOverlayDefinition?.IsDefault;
                        }
                    }

                    if (defaultFlowchartDefinition?.MediaHierarchyDefinition != null && defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels != null)
                    {
                        var defaultMediaHierarchyTemplateDetails = await portalDbContext.MediaHierarchyTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.MediaHierarchyDefinition.TemplateId && x.Removed == false);

                        defaultMediaHierarchyTemplateDetails = !isClientHasSameVersion && defaultMediaHierarchyTemplateDetails != null ? await GetMigratedDefinitionOfMediaHierarchyForPMDS(defaultMediaHierarchyTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : defaultMediaHierarchyTemplateDetails;

                        var currentMediaHierarchyTemplateDetails = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => defaultMediaHierarchyTemplateDetails != null && x.Name == defaultMediaHierarchyTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentMediaHierarchyTemplateDetails != null && !string.IsNullOrEmpty(currentMediaHierarchyTemplateDetails?.Name))
                        {
                            bool isMediaHierarchySame = defaultMediaHierarchyTemplateDetails.Version > currentMediaHierarchyTemplateDetails.Version;
                            if (isMediaHierarchySame)
                            {
                                if (defaultMediaHierarchyTemplateDetails != null)
                                {
                                    await CreateMediaHierarchyForDefaultTemplate(defaultFlowchartDefinition, defaultMediaHierarchyTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);
                                    if (currentMediaHierarchyTemplateDetails != null)
                                    {
                                        UpdateTemplateDetails(defaultMediaHierarchyTemplateDetails, currentMediaHierarchyTemplateDetails, defaultFlowchartDefinition);
                                    }
                                }
                            }
                            else
                            {
                                defaultFlowchartDefinition.MediaHierarchyDefinition.Definition = Json.Deserialize<MediaHierarchyDefinition>(currentMediaHierarchyTemplateDetails?.MediaHierarchyDefinition);
                            }
                            UpdateTemplate<MediaHierarchyTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentMediaHierarchyTemplateDetails);
                        }
                        else
                        {
                            await CreateMediaHierarchyForDefaultTemplate(defaultFlowchartDefinition, defaultMediaHierarchyTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);
                            await CreateDefaultTemplateMediaHierarchy(defaultFlowchartDefinition, currentFlowchartDefinition, defaultMediaHierarchyTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.MediaHierarchyDefinition = defaultFlowchartDefinition?.MediaHierarchyDefinition;
                        if (currentFlowchartDefinition.MediaHierarchyDefinition != null)
                        {
                            currentFlowchartDefinition.MediaHierarchyDefinition.TemplateVersion = defaultFlowchartDefinition?.MediaHierarchyDefinition?.TemplateVersion;
                            currentFlowchartDefinition.MediaHierarchyDefinition.IsDefault = defaultFlowchartDefinition?.MediaHierarchyDefinition?.IsDefault;
                        }
                    }

                    if (defaultFlowchartDefinition?.TotalsDefinition != null)
                    {
                        var defaultTotalsTemplateDetails = await portalDbContext.TotalsTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.TotalsDefinition.TemplateId && x.Removed == false);

                        defaultTotalsTemplateDetails = !isClientHasSameVersion && defaultTotalsTemplateDetails != null ? await GetMigratedDefinitionOfRightHandTotalForPMDS(defaultTotalsTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : defaultTotalsTemplateDetails;

                        var currentTotalsTemplateDetails = await portalDbContext.TotalsTemplates.FirstOrDefaultAsync(x => defaultTotalsTemplateDetails != null && x.Name == defaultTotalsTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentTotalsTemplateDetails != null && !string.IsNullOrWhiteSpace(currentTotalsTemplateDetails?.Name))
                        {
                            bool isTotalsSame = defaultTotalsTemplateDetails.Version > currentTotalsTemplateDetails.Version;
                            if (isTotalsSame)
                            {
                                if (defaultTotalsTemplateDetails != null && currentTotalsTemplateDetails != null)
                                {
                                    UpdateTemplateDetails(defaultTotalsTemplateDetails, currentTotalsTemplateDetails);
                                }
                            }
                            UpdateTemplate<TotalsTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentTotalsTemplateDetails);
                        }
                        else
                        {
                            await CreateDefaultTemplateRightHandTotal(defaultFlowchartDefinition, currentFlowchartDefinition, defaultTotalsTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.TotalsDefinition = defaultFlowchartDefinition?.TotalsDefinition;
                        if (currentFlowchartDefinition.TotalsDefinition != null)
                        {
                            currentFlowchartDefinition.TotalsDefinition.TemplateVersion = defaultFlowchartDefinition?.TotalsDefinition?.TemplateVersion;
                            currentFlowchartDefinition.TotalsDefinition.IsDefault = defaultFlowchartDefinition?.TotalsDefinition?.IsDefault;
                        }
                    }

                    if (defaultFlowchartDefinition?.GrandTotalDefinition != null)
                    {
                        var defaultGrandTotalTemplateDetails = await portalDbContext.GrandTotalTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.GrandTotalDefinition.TemplateId && x.Removed == false);

                        defaultGrandTotalTemplateDetails = !isClientHasSameVersion && defaultGrandTotalTemplateDetails != null ? await GetMigratedDefinitionOfGrandTotalForPMDS(defaultGrandTotalTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : defaultGrandTotalTemplateDetails;

                        var currentGrandTotalTemplateDetails = await portalDbContext.GrandTotalTemplates.FirstOrDefaultAsync(x => defaultGrandTotalTemplateDetails != null && x.Name == defaultGrandTotalTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentGrandTotalTemplateDetails != null && !string.IsNullOrWhiteSpace(currentGrandTotalTemplateDetails?.Name))
                        {
                            bool isGrandTotalSame = defaultGrandTotalTemplateDetails.Version > currentGrandTotalTemplateDetails.Version;
                            if (isGrandTotalSame)
                            {
                                if (defaultGrandTotalTemplateDetails != null && currentGrandTotalTemplateDetails != null)
                                {
                                    UpdateTemplateDetails(defaultGrandTotalTemplateDetails, currentGrandTotalTemplateDetails);
                                }
                            }
                            UpdateTemplate<GrandTotalTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentGrandTotalTemplateDetails);
                        }
                        else
                        {
                            await CreateDefaultTemplateGrandTotal(defaultFlowchartDefinition, currentFlowchartDefinition, defaultGrandTotalTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.GrandTotalDefinition = defaultFlowchartDefinition?.GrandTotalDefinition;
                        if (currentFlowchartDefinition.GrandTotalDefinition != null)
                        {
                            currentFlowchartDefinition.GrandTotalDefinition.TemplateVersion = defaultFlowchartDefinition?.GrandTotalDefinition?.TemplateVersion;
                            currentFlowchartDefinition.GrandTotalDefinition.IsDefault = defaultFlowchartDefinition?.GrandTotalDefinition?.IsDefault;
                        }

                    }

                    if (defaultFlowchartDefinition?.ThemeDefinition != null)
                    {
                        var defaultThemeTemplateDetails = await portalDbContext.ThemeTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.ThemeDefinition.TemplateId && x.Removed == false);

                        defaultThemeTemplateDetails = !isClientHasSameVersion && defaultThemeTemplateDetails != null ? await GetMigratedDefinitionOfThemeForPMDS(defaultThemeTemplateDetails, defaultClientMappingDetails.Version, mappingDetails, duplicates, cancellationToken) : defaultThemeTemplateDetails;

                        var currentThemeTemplateDetails = await portalDbContext.ThemeTemplates.FirstOrDefaultAsync(x => defaultThemeTemplateDetails != null && x.Name == defaultThemeTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentThemeTemplateDetails != null && !string.IsNullOrWhiteSpace(currentThemeTemplateDetails?.Name))
                        {
                            bool isThemeSame = defaultThemeTemplateDetails.Version > currentThemeTemplateDetails.Version;
                            if (isThemeSame)
                            {
                                if (defaultThemeTemplateDetails != null && currentThemeTemplateDetails != null)
                                {
                                    await CreateThemeForDefaultTemplate(defaultFlowchartDefinition, defaultThemeTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);

                                    UpdateTemplateDetails(defaultThemeTemplateDetails, currentThemeTemplateDetails, defaultFlowchartDefinition);
                                }
                            }
                            else
                            {
                                defaultFlowchartDefinition.ThemeDefinition.Definition = Json.Deserialize<ThemeDefinition>(currentThemeTemplateDetails?.ThemeDefinition);
                            }
                            UpdateTemplate<ThemeTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentThemeTemplateDetails);
                        }
                        else
                        {
                            await CreateThemeForDefaultTemplate(defaultFlowchartDefinition, defaultThemeTemplateDetails, ansid, omniClientId, calStartDate, calEndDate, omniClientInfoDTO, cancellationToken);

                            await CreateDefaultTemplateTheme(defaultFlowchartDefinition, currentFlowchartDefinition, defaultThemeTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.ThemeDefinition = defaultFlowchartDefinition?.ThemeDefinition;
                        if (currentFlowchartDefinition.ThemeDefinition != null)
                        {
                            currentFlowchartDefinition.ThemeDefinition.TemplateVersion = defaultFlowchartDefinition?.ThemeDefinition?.TemplateVersion;
                            currentFlowchartDefinition.ThemeDefinition.IsDefault = defaultFlowchartDefinition?.ThemeDefinition?.IsDefault;
                        }
                    }

                    if (defaultFlowchartDefinition?.FooterDefinition != null)
                    {
                        var defaultFooterTemplateDetails = await portalDbContext.FooterTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == defaultFlowchartDefinition.FooterDefinition.TemplateId && x.Removed == false);
                        var currentFooterTemplateDetails = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => defaultFooterTemplateDetails != null && x.Name == defaultFooterTemplateDetails.Name && x.OmniClientId == omniClientId && x.Removed == false);

                        if (currentFooterTemplateDetails != null && !string.IsNullOrWhiteSpace(currentFooterTemplateDetails?.Name))
                        {
                            bool isFooterSame = defaultFooterTemplateDetails.Version > currentFooterTemplateDetails.Version;
                            if (isFooterSame)
                            {
                                if (defaultFooterTemplateDetails != null && currentFooterTemplateDetails != null)
                                {
                                    UpdateTemplateDetails(defaultFooterTemplateDetails, currentFooterTemplateDetails);
                                }
                            }
                            UpdateTemplate<FooterTemplate>(defaultFlowchartDefinition, currentFlowchartDefinition, currentFooterTemplateDetails);
                        }
                        else
                        {
                            await CreateDefaultTemplateFooter(defaultFlowchartDefinition, currentFlowchartDefinition, defaultFooterTemplateDetails, defaultUserId, omniClientId, cancellationToken);
                        }
                    }
                    else
                    {
                        currentFlowchartDefinition.FooterDefinition = defaultFlowchartDefinition?.FooterDefinition;
                        if (currentFlowchartDefinition.FooterDefinition != null)
                        {
                            currentFlowchartDefinition.FooterDefinition.TemplateVersion = defaultFlowchartDefinition?.FooterDefinition?.TemplateVersion;
                            currentFlowchartDefinition.FooterDefinition.IsDefault = defaultFlowchartDefinition?.FooterDefinition?.IsDefault;
                        }
                    }

                    currentTemplate.FlowchartDefinition = Json.Serialize(currentFlowchartDefinition);
                    currentTemplate.Version = defautTemplate.Version;
                    currentTemplate.IsDefault = defautTemplate.IsDefault;
                    currentTemplate.ModifiedDate = DateTime.UtcNow;

                    await portalDbContext.SaveChangesAsync(cancellationToken);
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, $"BackgroundJob: Error updating default template {name} with omniguidId {omniClientId}.", ex);
                return false;
            }
        }

        public void UpdateTemplate<T>(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, T template) where T : NamedDbModelBase
        {
            if (updateTemplateActions.TryGetValue(typeof(T), out var updateAction))
            {
                updateAction(defaultFlowchart, currentFlowchart, template);
            }
            else
            {
                throw new ArgumentException("Unsupported template type");
            }
        }

        private void UpdateHeaderTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, HeaderTemplate template)
        {
            currentFlowchart.HeaderDefinition = defaultFlowchart?.HeaderDefinition;
            currentFlowchart.HeaderDefinition.TemplateId = template.Id;
            currentFlowchart.HeaderDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.HeaderDefinition.IsDefault = defaultFlowchart?.HeaderDefinition?.IsDefault ?? false;
        }

        private void UpdateCalendarTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, CalendarTemplate template)
        {
            currentFlowchart.CalendarDefinition = defaultFlowchart?.CalendarDefinition;
            currentFlowchart.CalendarDefinition.TemplateId = template.Id;
            currentFlowchart.CalendarDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.CalendarDefinition.IsDefault = defaultFlowchart?.CalendarDefinition?.IsDefault ?? false;
        }

        private void UpdateCalendarOverlayTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, CalendarOverlayTemplate template)
        {
            currentFlowchart.CalendarOverlayDefinition = defaultFlowchart?.CalendarOverlayDefinition;
            currentFlowchart.CalendarOverlayDefinition.TemplateId = template.Id;
            currentFlowchart.CalendarOverlayDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.CalendarOverlayDefinition.IsDefault = defaultFlowchart?.CalendarOverlayDefinition?.IsDefault ?? false;
        }

        private void UpdateMediaHierarchyTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, MediaHierarchyTemplate template)
        {
            currentFlowchart.MediaHierarchyDefinition = defaultFlowchart?.MediaHierarchyDefinition;
            currentFlowchart.MediaHierarchyDefinition.TemplateId = template.Id;
            currentFlowchart.MediaHierarchyDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.MediaHierarchyDefinition.IsDefault = defaultFlowchart?.MediaHierarchyDefinition?.IsDefault ?? false;

        }

        private void UpdateGrandTotalTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, GrandTotalTemplate template)
        {
            currentFlowchart.GrandTotalDefinition = defaultFlowchart?.GrandTotalDefinition;
            currentFlowchart.GrandTotalDefinition.TemplateId = template.Id;
            currentFlowchart.GrandTotalDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.GrandTotalDefinition.IsDefault = defaultFlowchart?.GrandTotalDefinition?.IsDefault ?? false;

        }

        private void UpdateTotalsTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, TotalsTemplate template)
        {
            currentFlowchart.TotalsDefinition = defaultFlowchart?.TotalsDefinition;
            currentFlowchart.TotalsDefinition.TemplateId = template.Id;
            currentFlowchart.TotalsDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.TotalsDefinition.IsDefault = defaultFlowchart?.TotalsDefinition?.IsDefault ?? false;

        }

        private void UpdateThemeTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, ThemeTemplate template)
        {
            currentFlowchart.ThemeDefinition = defaultFlowchart?.ThemeDefinition;
            currentFlowchart.ThemeDefinition.TemplateId = template.Id;
            currentFlowchart.ThemeDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.ThemeDefinition.IsDefault = defaultFlowchart?.ThemeDefinition?.IsDefault ?? false;
        }

        private void UpdateFooterTemplate(FlowchartDefinition defaultFlowchart, FlowchartDefinition currentFlowchart, FooterTemplate template)
        {
            currentFlowchart.FooterDefinition = defaultFlowchart?.FooterDefinition;
            currentFlowchart.FooterDefinition.TemplateId = template.Id;
            currentFlowchart.FooterDefinition.TemplateVersion = template.Version != 0 ? template.Version : 1;
            currentFlowchart.FooterDefinition.IsDefault = defaultFlowchart?.FooterDefinition?.IsDefault ?? false;

        }

        private async Task<HeaderTemplate> CreateDefaultTemplateHeader(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, HeaderTemplate headerTemplateDetails, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var ht = new HeaderTemplate
            {
                Name = headerTemplateDetails.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, headerTemplateDetails.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, headerTemplateDetails.ModifiedByUserId),
                OmniClientId = omniClientId,
                HeaderDefinition = headerTemplateDetails.HeaderDefinition,
                Version = headerTemplateDetails.Version,
                IsDefault = headerTemplateDetails.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            ht = await portal.HeaderTemplates.AddAsync(ht, cancellationToken);
            UpdateTemplate<HeaderTemplate>(flowchartDefinition, newflowchartDefinition, ht);
            return ht;
        }

        private async Task<CalendarTemplate> CreateDefaultTemplateCalendar(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, CalendarTemplate calendarTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new CalendarTemplate
            {
                Name = calendarTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, calendarTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, calendarTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                CalendarDefinition = calendarTemplate.CalendarDefinition,
                Version = calendarTemplate.Version,
                IsDefault = calendarTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.CalendarTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<CalendarTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }

        private async Task<CalendarOverlayTemplate> CreateDefaultTemplateCalendarOverlay(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, CalendarOverlayTemplate calendarOverlayTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new CalendarOverlayTemplate
            {
                Name = calendarOverlayTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, calendarOverlayTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, calendarOverlayTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                CalendarOverlayDefinition = calendarOverlayTemplate.CalendarOverlayDefinition,
                Version = calendarOverlayTemplate.Version,
                IsDefault = calendarOverlayTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.CalendarOverlayTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<CalendarOverlayTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }


        private async Task<MediaHierarchyTemplate> CreateDefaultTemplateMediaHierarchy(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, MediaHierarchyTemplate mediaHierarchyTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new MediaHierarchyTemplate
            {
                Name = mediaHierarchyTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, mediaHierarchyTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, mediaHierarchyTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                MediaHierarchyDefinition = Json.Serialize(flowchartDefinition.MediaHierarchyDefinition.Definition),
                Currency = mediaHierarchyTemplate.Currency,
                DisplaySource = mediaHierarchyTemplate.DisplaySource,
                ShowSubTotalsAtBottom = mediaHierarchyTemplate.ShowSubTotalsAtBottom,
                Version = mediaHierarchyTemplate.Version,
                IsDefault = mediaHierarchyTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.MediaHierarchyTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<MediaHierarchyTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }


        private async Task<GrandTotalTemplate> CreateDefaultTemplateGrandTotal(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, GrandTotalTemplate grandTotalTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new GrandTotalTemplate
            {
                Name = grandTotalTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, grandTotalTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, grandTotalTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                GrandTotalDefinition = grandTotalTemplate.GrandTotalDefinition,
                Version = grandTotalTemplate.Version,
                IsDefault = grandTotalTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.GrandTotalTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<GrandTotalTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }


        private async Task<TotalsTemplate> CreateDefaultTemplateRightHandTotal(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, TotalsTemplate totalsTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new TotalsTemplate
            {
                Name = totalsTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, totalsTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, totalsTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                TotalsDefinition = totalsTemplate.TotalsDefinition,
                Version = totalsTemplate.Version,
                IsDefault = totalsTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.TotalsTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<TotalsTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }


        private async Task<ThemeTemplate> CreateDefaultTemplateTheme(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, ThemeTemplate themeTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new ThemeTemplate
            {
                Name = themeTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, themeTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, themeTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                ThemeDefinition = Json.Serialize(flowchartDefinition.ThemeDefinition.Definition),
                Version = themeTemplate.Version,
                IsDefault = themeTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.ThemeTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<ThemeTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }


        private async Task<FooterTemplate> CreateDefaultTemplateFooter(FlowchartDefinition flowchartDefinition, FlowchartDefinition newflowchartDefinition, FooterTemplate footerTemplate, Guid defaultUserId, Guid omniClientId, CancellationToken cancellationToken)
        {
            var newTemplate = new FooterTemplate
            {
                Name = footerTemplate.Name,
                CreatedByUserId = ChangeDefaultUser(defaultUserId, footerTemplate.CreatedByUserId),
                ModifiedByUserId = ChangeDefaultUser(defaultUserId, footerTemplate.ModifiedByUserId),
                OmniClientId = omniClientId,
                FooterDefinition = footerTemplate.FooterDefinition,
                Version = footerTemplate.Version,
                IsDefault = footerTemplate.IsDefault,
                ModifiedDate = DateTime.UtcNow,
                Removed = false,
            };
            newTemplate = await portal.FooterTemplates.AddAsync(newTemplate, cancellationToken);
            UpdateTemplate<FooterTemplate>(flowchartDefinition, newflowchartDefinition, newTemplate);
            return newTemplate;
        }

        private void UpdateTemplateDetails(HeaderTemplate defaultTemplate, HeaderTemplate currentTemplate)
        {
            currentTemplate.HeaderDefinition = defaultTemplate.HeaderDefinition;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        private void UpdateTemplateDetails(CalendarTemplate defaultTemplate, CalendarTemplate currentTemplate)
        {
            currentTemplate.CalendarDefinition = defaultTemplate.CalendarDefinition;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        private void UpdateTemplateDetails(CalendarOverlayTemplate defaultTemplate, CalendarOverlayTemplate currentTemplate)
        {
            currentTemplate.CalendarOverlayDefinition = defaultTemplate.CalendarOverlayDefinition;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        private void UpdateTemplateDetails(MediaHierarchyTemplate defaultTemplate, MediaHierarchyTemplate currentTemplate, FlowchartDefinition flowchartDefinition)
        {
            currentTemplate.MediaHierarchyDefinition = Json.Serialize(flowchartDefinition.MediaHierarchyDefinition.Definition);
            currentTemplate.Currency = defaultTemplate.Currency;
            currentTemplate.DisplaySource = defaultTemplate.DisplaySource;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ShowSubTotalsAtBottom = defaultTemplate.ShowSubTotalsAtBottom;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }

        private void UpdateTemplateDetails(GrandTotalTemplate defaultTemplate, GrandTotalTemplate currentTemplate)
        {
            currentTemplate.GrandTotalDefinition = defaultTemplate.GrandTotalDefinition;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        private void UpdateTemplateDetails(TotalsTemplate defaultTemplate, TotalsTemplate currentTemplate)
        {
            currentTemplate.TotalsDefinition = defaultTemplate.TotalsDefinition;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        private void UpdateTemplateDetails(ThemeTemplate defaultTemplate, ThemeTemplate currentTemplate, FlowchartDefinition flowchartDefinition)
        {
            currentTemplate.ThemeDefinition = Json.Serialize(flowchartDefinition.ThemeDefinition.Definition);
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        private void UpdateTemplateDetails(FooterTemplate defaultTemplate, FooterTemplate currentTemplate)
        {
            currentTemplate.FooterDefinition = defaultTemplate.FooterDefinition;
            currentTemplate.Version = defaultTemplate.Version;
            currentTemplate.IsDefault = defaultTemplate.IsDefault;
            currentTemplate.ModifiedDate = DateTime.UtcNow;
        }
        public async Task CreateThemeForDefaultTemplate(FlowchartDefinition? defaultFlowchartDefinition, ThemeTemplate defaultThemeTemplateDetails, string ansid, Guid omniClientId, DateTime calStartDate, DateTime calEndDate, OmniClientInfoDTO omniClientInfoDTO, CancellationToken cancellationToken)
        {
            // update code
            defaultFlowchartDefinition.ThemeDefinition.Definition = Json.Deserialize<ThemeDefinition>(defaultThemeTemplateDetails.ThemeDefinition);
            if (defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme != null && defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.ColumnName != null && defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.TableId != null)
            {
                var getDistinctDataDto = new DistinctDataDTO()
                {
                    ParentColumnName = null,
                    ParentSelectedValue = null,
                    ParentTableId = null,
                    OmniClientId = omniClientId,
                    TableId = (Guid)defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.TableId,
                    ColumnName = defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.ColumnName,
                    StartDate = calStartDate,
                    EndDate = calEndDate,
                };

                var dataMH = omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansid, defaultThemeTemplateDetails.CreatedByUserId, cancellationToken) : await controllerLogic.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, defaultThemeTemplateDetails.CreatedByUserId, cancellationToken);

                bool matchExists = defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.ToList().Any(x => dataMH.ToList().Contains(x.Name));
                var matched = defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.Where(x => dataMH.ToList().Contains(x.Name)).ToList();

                var isAnySettingSelected = matched.Any(c => c.Enabled);
                foreach (var setting in matched.Select((value, index) => new { value, index }))
                {
                    if (!isAnySettingSelected)
                    {
                        setting.value.Enabled = true;
                    }
                }

                defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings = !matchExists ? dataMH.OrderBy(x => x).Select((x, index) => new LegendThemeSetting()
                {
                    Name = x.ToString(),
                    InflightOverlayStyling = defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.ToList()[0].Styling,
                    Enabled = true,
                    Styling = defaultFlowchartDefinition.ThemeDefinition.Definition.LegendTheme.Settings.ToList()[0].Styling
                }).ToList() : matched;
            }
        }

        public async Task CreateMediaHierarchyForDefaultTemplate(FlowchartDefinition? defaultFlowchartDefinition, MediaHierarchyTemplate defaultMediaHierarchyTemplateDetails, string ansid, Guid omniClientId, DateTime calStartDate, DateTime calEndDate, OmniClientInfoDTO omniClientInfoDTO, CancellationToken cancellationToken)
        {
            defaultFlowchartDefinition.MediaHierarchyDefinition.Definition = Json.Deserialize<MediaHierarchyDefinition>(defaultMediaHierarchyTemplateDetails?.MediaHierarchyDefinition);
            for (int i = 0; i < defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.Count; i++)
            {
                var levelData = await GetPreviousLevelsData(defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels, i);

                var getDistinctDataDto = new DistinctDataDTO()
                {
                    ParentColumnName = null,
                    ParentSelectedValue = null,
                    ParentTableId = null,
                    Levels = levelData,
                    OmniClientId = omniClientId,
                    TableId = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].TableId,
                    ColumnName = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].ColumnName,
                    StartDate = calStartDate,
                    EndDate = calEndDate,
                };

                var dataMH = omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansid, defaultMediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken) : await controllerLogic.GetDistinctDataAsync(getDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, defaultMediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken);

                Guid metricTableId = omniClientInfoDTO.Version > 1 ? DefaultMetricTableIdPMDS : DefaultMetricTableId;

                bool matchExists = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList().Any(x => dataMH.ToList().Contains(x.Name));
                var matched = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.Where(x => dataMH.ToList().Contains(x.Name)).ToList();

                var isAnySettingSelected = matched.Any(c => c.Enabled);
                foreach (var setting in matched.Select((value, index) => new { value, index }))
                {
                    if (!isAnySettingSelected)
                    {
                        setting.value.Enabled = true;
                    }
                    setting.value.Order = setting.index;
                }

                defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings = !matchExists ? dataMH.OrderBy(x => x).Select((x, index) => new MediaHierarchySetting()
                {
                    Name = x.ToString(),
                    FlightRange = FlightRange.FlightTotal,
                    InflightOverlayStyling = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[0].Styling,
                    Enabled = true,
                    MetricTableId = metricTableId,
                    MetricColumnName = DefaultMetricColumnName,
                    Order = index,
                    Styling = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[0].Styling
                }).ToList() : matched;

                for (int j = 0; j < defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels?.ToList()[i].Settings.ToList().Count; j++)
                {
                    if (defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels?.ToList()?.Count() > 0)
                    {
                        for (int k = 0; k < defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings?.ToList()[j].SubLevels?.ToList().Count; k++)
                        {
                            var sublevelData = await GetPreviousLevelsData(defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels, i, defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels, k);

                            if (sublevelData.Count > 0)
                            {
                                var currentSubLevel = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k];
                                var getSubLevelDistinctDataDto = new DistinctDataDTO()
                                {
                                    ParentColumnName = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].ColumnName,
                                    ParentSelectedValue = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings?.ToList()[j].Name,
                                    ParentTableId = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].TableId,
                                    Levels = sublevelData,
                                    OmniClientId = omniClientId,
                                    TableId = currentSubLevel.TableId,
                                    ColumnName = currentSubLevel.ColumnName,
                                    StartDate = calStartDate,
                                    EndDate = calEndDate,
                                };

                                var sublevelDataMH = omniClientInfoDTO.Version > 1 ? await dataControllerLogicPmds.GetDistinctDataAsync(getSubLevelDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, ansid, defaultMediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken) : await controllerLogic.GetDistinctDataAsync(getSubLevelDistinctDataDto, omniClientInfoDTO.Client.IncludeSourceMediaToolsData, defaultMediaHierarchyTemplateDetails.CreatedByUserId, cancellationToken);

                                Guid subLevelMetricTableId = omniClientInfoDTO.Version > 1 ? DefaultMetricTableIdPMDS : DefaultMetricTableId;

                                bool subLevelMatchExists = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings.ToList().Any(x => sublevelDataMH.ToList().Contains(x.Name));

                                var subLevelMatched = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings.Where(x => sublevelDataMH.ToList().Contains(x.Name)).ToList();

                                var isAnySettingSelectedAtSublevel = subLevelMatched.Any(c => c.Enabled);

                                foreach (var setting in subLevelMatched.Select((value, index) => new { value, index }))
                                {
                                    if (!isAnySettingSelectedAtSublevel)
                                    {
                                        setting.value.Enabled = true;
                                    }
                                    setting.value.Order = setting.index;
                                }

                                defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings = !subLevelMatchExists ? sublevelDataMH.OrderBy(x => x).Select((x, index) => new MediaHierarchySubLevelSetting()
                                {
                                    Name = x.ToString(),
                                    FlightRange = FlightRange.FlightTotal,
                                    InflightOverlayStyling = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[0].Styling,
                                    Enabled = true,
                                    MetricTableId = subLevelMetricTableId,
                                    MetricColumnName = DefaultMetricColumnName,
                                    Order = index,
                                    Styling = defaultFlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ToList()[i].Settings.ToList()[j].SubLevels.ToList()[k].Settings.ToList()[0].Styling
                                }).ToList() : subLevelMatched;

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get All default flowchart template list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<FlowchartTemplate>> GetAllDefaultFlowchartTemplateListAsync(CancellationToken cancellationToken)
        {
            var defaultTemplatesList = await portalDbContext.DefaultTemplates.Where(dt => !dt.Removed)
                .Join(
                      portalDbContext.FlowchartTemplates.Where(ft => !ft.Removed),
                      dt => new { dt.OmniClientId, dt.Name },
                      ft => new { ft.OmniClientId, ft.Name },
                      (dt, ft) => ft
                     ).AsNoTracking().ToListAsync(cancellationToken);
            return defaultTemplatesList;
        }

        /// <summary>
        /// Update the default template for all clients asynchronous.
        /// </summary>
        /// <param name="ansid">The ansid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="isUpdate">The isUpdate.</param>
        public async Task UpdateDefaultTemplateForAllClientAsync(string ansid, CancellationToken cancellationToken, Guid? omniClientId, bool isUpdate = false)
        {
            try
            {
                log.Add(LogLevel.Information, $"Update default template started.");

                var defaultTemplatesList = await GetAllDefaultFlowchartTemplateListAsync(cancellationToken);


                foreach (var defaultTemplate in defaultTemplatesList)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    log.Add(LogLevel.Information, $"Update default template started for template name: {defaultTemplate.Name}");
                    bool isSuccess = await UpdateDefaultTemplateAsync(defaultTemplate, ansid, cancellationToken, omniClientId, isUpdate);
                    sw.Stop();
                    log.Add(LogLevel.Information, $"Update default template finished for template name: {defaultTemplate.Name} in {sw.Elapsed.TotalMinutes}");
                    if (isSuccess)
                    {
                        log.Add(LogLevel.Information, $"Update default template finished for template name: {defaultTemplate.Name}");
                    }
                    else
                    {
                        log.Add(LogLevel.Information, $"Update default template failed for template name: {defaultTemplate.Name}");
                    }
                }

                // tried below and it throws the thread exception in dbcontext
                //await Parallel.ForEachAsync(defaultTemplatesList, async (defaultTemplates, token) =>
                //{
                //    bool isSuccess = await UpdateDefaultTemplateAsync(defaultTemplates, ansid, token, omniClientId, isUpdate);
                //    if (isSuccess)
                //    {
                //        log.Add(LogLevel.Information, $"Update default template finished for template name: {defaultTemplates.Name}");
                //    }
                //    else
                //    {
                //        log.Add(LogLevel.Information, $"Update default template failed for template name: {defaultTemplates.Name}");
                //    }
                //});
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, $"Error updating default template", ex);
            }
        }

        /// <summary>
        /// Get Default FlowchartTemplate list By Template Name asynchronous.
        /// </summary>
        /// <param name="templateName">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<SharedFlowchartTemplateDetails>> GetFlowchartTemplateListByTemplateNameAsync(string templateName, CancellationToken cancellationToken)
        {
            var query = await (from c in portalDbContext.Clients
                               join oc in portalDbContext.OmniClients on c.Id equals oc.ClientId
                               join cm in portalDbContext.ClientMapping on c.Id equals cm.ClientId
                               join f in portalDbContext.FlowchartTemplates on oc.Id equals f.OmniClientId
                               join u in portalDbContext.Users on f.CreatedByUserId equals u.Id
                               where f.Name == templateName
                               select new SharedFlowchartTemplateDetails
                               {
                                   Id = f.Id,
                                   Name = f.Name,
                                   CreatedByUser = u.DisplayName,
                                   ClientName = c.Name,
                                   OmniClientName = c.ClientId,
                                   OmniClientId = oc.Id,
                                   CreatedByUserId = f.CreatedByUserId,
                                   ModifiedByUserId = f.ModifiedByUserId,
                                   CreatedDate = f.CreatedDate,
                                   ModifiedDate = f.ModifiedDate,
                                   ClientMappingClientId = cm.ClientId,
                                   Version = cm.Version,
                                   Removed = cm.Removed,
                               }).AsNoTracking().ToListAsync(cancellationToken);
            return query;
        }

        /// <summary>
        /// GetDefaultTemplateUser
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<Guid> GetDefaultTemplateUser(CancellationToken cancellationToken)
        {
            try
            {
                var user = await portal.Users.GetByNameAsync(defaultTemplateConfig.CurrentValue.UserEmail, cancellationToken);
                return user.Id;
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, "Default user doesn't exists", ex);
            }
            return Guid.Empty;
        }

        private static Guid ChangeDefaultUser(Guid defaultuserId, Guid templateUserId)
        {
            return defaultuserId == Guid.Empty ? templateUserId : defaultuserId;
        }

        private async Task<FlowchartTemplate> GetMigratedDefinitionOfFlowchartForPMDS(FlowchartTemplate template, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var flowcharts = new FlowchartDefinitionDetailsDTO()
            {
                Id = template.Id,
                Definition = template.FlowchartDefinition == null ? null : Json.Deserialize<FlowchartDefinition>(template.FlowchartDefinition),
                CreatedByUserId = template.CreatedByUserId
            };
            return await migrationControllerLogic.GetUpdatedFlowchartForPMDS(flowcharts, template, clientversion, mappingDetails, duplicates, cancellationToken, template.OmniClientId);
        }

        private async Task<HeaderTemplate> GetMigratedDefinitionOfHeaderForPMDS(HeaderTemplate template, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var header = new HeaderDefinitionDetailsDTO()
            {
                Id = template.Id,
                Definition = template.HeaderDefinition == null ? null : Json.Deserialize<HeaderDefinition>(template.HeaderDefinition),
                CreatedByUserId = template.CreatedByUserId,
            };
            return await migrationControllerLogic.GetUpdatedHeaderForPMDS(header, template, clientversion, mappingDetails, duplicates, cancellationToken,template.OmniClientId);
        }

        private async Task<MediaHierarchyTemplate> GetMigratedDefinitionOfMediaHierarchyForPMDS(MediaHierarchyTemplate template, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var mediaHierarchy = new MediaHierarchyDefinitionDetailsDTO()
            {
                Id = template.Id,
                Definition = template.MediaHierarchyDefinition == null ? null : Json.Deserialize<MediaHierarchyDefinition>(template.MediaHierarchyDefinition),
                CreatedByUserId = template.CreatedByUserId,
            };
            return await migrationControllerLogic.GetUpdatedMediaHierarchyForPMDS(mediaHierarchy, template, clientversion, mappingDetails, duplicates, cancellationToken, template.OmniClientId);
        }

        private async Task<TotalsTemplate> GetMigratedDefinitionOfRightHandTotalForPMDS(TotalsTemplate template, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var themes = new RightHandTotalsDefinitionDetailsDTO()
            {
                Id = template.Id,
                Definition = template.TotalsDefinition == null ? null : Json.Deserialize<TotalsDefinition>(template.TotalsDefinition),
                CreatedByUserId = template.CreatedByUserId,
            };
            return await migrationControllerLogic.GetUpdatedRightHandTotalsForPMDS(themes, template, clientversion, mappingDetails, duplicates, cancellationToken, template.OmniClientId);
        }

        private async Task<GrandTotalTemplate> GetMigratedDefinitionOfGrandTotalForPMDS(GrandTotalTemplate template, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var themes = new GrandTotalDefintionDetailsDTO()
            {
                Id = template.Id,
                Definition = template.GrandTotalDefinition == null ? null : Json.Deserialize<GrandTotalDefinition>(template.GrandTotalDefinition),
                CreatedByUserId = template.CreatedByUserId,
            };
            return await migrationControllerLogic.GetUpdatedGrandTotalForPMDS(themes, template, clientversion, mappingDetails, duplicates, cancellationToken, template.OmniClientId);
        }

        private async Task<ThemeTemplate> GetMigratedDefinitionOfThemeForPMDS(ThemeTemplate template, int clientversion, List<PlannedMediaMigrationColumnMapping> mappingDetails, List<string>? duplicates, CancellationToken cancellationToken)
        {
            var themes = new ThemeDefinitionDetailsDTO()
            {
                Id = template.Id,
                Definition = template.ThemeDefinition == null ? null : Json.Deserialize<ThemeDefinition>(template.ThemeDefinition),
                CreatedByUserId = template.CreatedByUserId,
            };
            return await migrationControllerLogic.GetUpdatedThemeTemplateForPMDS(themes, template, clientversion, mappingDetails, duplicates, cancellationToken, template.OmniClientId);
        }
    }
}