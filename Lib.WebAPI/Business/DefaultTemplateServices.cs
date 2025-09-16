using AutoMapper;
using DocumentFormat.OpenXml.Office.Word;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Models.Flowchart;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DefaultTemplateServices
    /// </summary>
    public class DefaultTemplateServices : IDefaultTemplateServices
    {
        private readonly IPortalUnitOfWork portal;
        private readonly ILog<IDefaultTemplateServices> log;
        private readonly IMapper mapper;
        private readonly IPortalDbContext portalDbContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTemplateServices"/> class.
        /// The FieldInfoServices constructor
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        public DefaultTemplateServices(IPortalUnitOfWork portal, ILog<IDefaultTemplateServices> log, IMapper mapper, IPortalDbContext portalDbContext)
        {
            this.portal = portal;
            this.log = log;
            this.mapper = mapper;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Create the default templates asynchronous.
        /// </summary>
        /// <param name="createDefaultTemplates">The createDefaultTemplates Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CreateDefaultTemplateAsync(List<DefaultTemplateDTO> createDefaultTemplates, CancellationToken cancellationToken)
        {
            // Mapping the createDefaultTemplates to DefaultTemplate
            createDefaultTemplates.ThrowIfNull(nameof(createDefaultTemplates));
            var defaultTemplateDetails = await portalDbContext.DefaultTemplates.ToListAsync();

            var dataInsert = defaultTemplateDetails.Count > 0 ? createDefaultTemplates.Where(x => !defaultTemplateDetails.Any(y => y.Name == x.Name && y.OmniClientId == x.OmniClientId && y.Removed == x.Removed)).ToList() : createDefaultTemplates;

            if (dataInsert.Count > 0)
            {
                var defaultTemplateDetailsDto = mapper.Map<List<DefaultTemplate>>(dataInsert);
                await portal.DefaultTemplates.AddRangeAsync(defaultTemplateDetailsDto, cancellationToken);

                foreach (var item in defaultTemplateDetailsDto)
                {
                    bool isSet = await setIsDefaultForFlowchartAndComponents(item, cancellationToken, true);
                    if (isSet)
                    {
                        log.Add(LogLevel.Information, $"Created : IsDefault value is set for Flowchart template with name : {item.Name} and their components with omniGuid : {item.OmniClientId}");
                    }
                    else
                    {
                        log.Add(LogLevel.Error, $"Create : Error while setting IsDefault for flowchart and components for default template name : {item.Name} with omniGuid : {item.OmniClientId}");
                    }
                }
                await portal.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the default Template asynchronous.
        /// </summary>
        /// <param name="updateDefaultTemplate">The updateDefaultTemplate Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> UpdateDefaultTemplateAsync(UpdateDefaultTemplateDTO updateDefaultTemplate, CancellationToken cancellationToken)
        {
            bool isUpdated = false;
            updateDefaultTemplate.ThrowIfNull(nameof(updateDefaultTemplate));
            var defaultTemplateDetails = await portalDbContext.DefaultTemplates.FirstOrDefaultAsync(x => x.Id == updateDefaultTemplate.Id, cancellationToken: cancellationToken);
            defaultTemplateDetails.ThrowIfNull(nameof(defaultTemplateDetails));

            isUpdated = (defaultTemplateDetails.Name != updateDefaultTemplate.Name || defaultTemplateDetails.OmniClientId != updateDefaultTemplate.OmniClientId || defaultTemplateDetails.Removed != updateDefaultTemplate.Removed) ? true : false;

            defaultTemplateDetails.Name = updateDefaultTemplate.Name;
            defaultTemplateDetails.OmniClientId = updateDefaultTemplate.OmniClientId;
            defaultTemplateDetails.Removed = updateDefaultTemplate.Removed;

            bool setIsdefaultValue = updateDefaultTemplate.Removed == false ? true : false;

            bool isSet = await setIsDefaultForFlowchartAndComponents(defaultTemplateDetails, cancellationToken, setIsdefaultValue);
            if (isSet)
            {
                log.Add(LogLevel.Information, $"Updated : IsDefault value is set for Flowchart template with name : {updateDefaultTemplate.Name} and their components with omniGuid : {updateDefaultTemplate.OmniClientId}");
            }
            else
            {
                log.Add(LogLevel.Error, $"Update : Error while setting IsDefault for flowchart and components for default template name : {updateDefaultTemplate.Name} with omniGuid : {updateDefaultTemplate.OmniClientId}");
            }

            await portal.SaveChangesAsync(cancellationToken);
            return isUpdated;
        }

        /// <summary>
        /// Gets the default template details list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        public async Task<List<DefaultTemplate>> GetAllAsync(CancellationToken cancellationToken, bool removed = false)
        {
            var defaultTemplateDetails = await portal.DefaultTemplates.GetAllAsync(cancellationToken, removed);
            defaultTemplateDetails.ThrowIfNull(nameof(defaultTemplateDetails));
            return (List<DefaultTemplate>)defaultTemplateDetails;
        }


        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var defaultTemplateDetails = await portal.DefaultTemplates.GetByIdAsync(id, cancellationToken);

            log.Add(LogLevel.Information, $"Deleting default template record: {defaultTemplateDetails.Id}");

            await portal.DefaultTemplates.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> setIsDefaultForFlowchartAndComponents(DefaultTemplate defaultTemplate, CancellationToken cancellationToken, bool isCreated = false)
        {
            var flowchartTemplate = await portalDbContext.FlowchartTemplates.FirstOrDefaultAsync(x => x.OmniClientId == defaultTemplate.OmniClientId && x.Name == defaultTemplate.Name, cancellationToken);
            flowchartTemplate.ThrowIfNull($"Flowchart name : {defaultTemplate.Name} not exits with omniGuid : {defaultTemplate.OmniClientId} and default template cannot be created", nameof(flowchartTemplate));
            FlowchartDefinition? flowchartDefinition = Json.Deserialize<FlowchartDefinition>(flowchartTemplate.FlowchartDefinition);

            if (flowchartDefinition.HeaderDefinition != null)
            {
                var headerTemplateDetails = await portalDbContext.HeaderTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.HeaderDefinition.TemplateId && x.Removed == false);
                if (headerTemplateDetails != null)
                {
                    headerTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.HeaderDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.CalendarDefinition != null)
            {
                var calendarTemplateDetails = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarDefinition.TemplateId && x.Removed == false);
                if (calendarTemplateDetails != null)
                {
                    calendarTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.CalendarDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.CalendarOverlayDefinition != null)
            {
                var calendarOverlayTemplateDetails = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.CalendarOverlayDefinition.TemplateId && x.Removed == false);
                if (calendarOverlayTemplateDetails != null)
                {
                    calendarOverlayTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.CalendarOverlayDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.MediaHierarchyDefinition != null && flowchartDefinition.MediaHierarchyDefinition.Definition.Levels != null)
            {
                var mediaHierarchyTemplateDetails = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.MediaHierarchyDefinition.TemplateId && x.Removed == false);
                if (mediaHierarchyTemplateDetails != null)
                {
                    mediaHierarchyTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.MediaHierarchyDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.GrandTotalDefinition != null)
            {
                var grandTotalTemplateDetails = await portalDbContext.GrandTotalTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.GrandTotalDefinition.TemplateId && x.Removed == false);
                if (grandTotalTemplateDetails != null)
                {
                    grandTotalTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.GrandTotalDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.TotalsDefinition != null)
            {
                var rightHandTotalsTemplateDetails = await portalDbContext.TotalsTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.TotalsDefinition.TemplateId && x.Removed == false);
                if (rightHandTotalsTemplateDetails != null)
                {
                    rightHandTotalsTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.TotalsDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.ThemeDefinition != null)
            {
                var themeTemplateDetails = await portalDbContext.ThemeTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.ThemeDefinition.TemplateId && x.Removed == false);
                if (themeTemplateDetails != null)
                {
                    themeTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.ThemeDefinition.IsDefault = isCreated;
                }
            }

            if (flowchartDefinition.FooterDefinition != null)
            {
                var footerTemplateDetails = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => x.Id == flowchartDefinition.FooterDefinition.TemplateId && x.Removed == false);
                if (footerTemplateDetails != null)
                {
                    footerTemplateDetails.IsDefault = isCreated;
                    flowchartDefinition.FooterDefinition.IsDefault = isCreated;
                }
            }
            flowchartTemplate.IsDefault = isCreated;
            await portal.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <summary>
        /// Check the create or update Default Component async.
        /// </summary>
        /// <param name="createUpdateDefaultComponentDTO">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CreateUpdateDefaultComponentAsync(CreateUpdateDefaultComponentDTO createUpdateDefaultComponentDTO, CancellationToken cancellationToken)
        {
            bool updated = false;
            createUpdateDefaultComponentDTO.ThrowIfNull(nameof(createUpdateDefaultComponentDTO));
            createUpdateDefaultComponentDTO.ComponentName.ThrowIfNull(nameof(createUpdateDefaultComponentDTO.ComponentName));

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.Header)
            {
                var template = await portalDbContext.HeaderTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.Calendar)
            {
                var template = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.CalendarOverlay)
            {
                var template = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.MediaHierarchy)
            {
                var template = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.GrandTotals)
            {
                var template = await portalDbContext.GrandTotalTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.RightHandTotals)
            {
                var template = await portalDbContext.TotalsTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.Footer)
            {
                var template = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.Theme)
            {
                var template = await portalDbContext.ThemeTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }

            if (createUpdateDefaultComponentDTO.ComponentName == FlowChartComponent.None)
            {
                var template = await portalDbContext.FlowchartTemplates.FirstOrDefaultAsync(x => x.Id == createUpdateDefaultComponentDTO.TemplateId && x.OmniClientId == createUpdateDefaultComponentDTO.OmniClientId, cancellationToken);
                template.ThrowIfNull(nameof(template));
                template.IsDefault = createUpdateDefaultComponentDTO.IsDefault;
                updated = template.IsDefault;
            }
            await portalDbContext.SaveChangesAsync(cancellationToken);
            return updated;
        }
    }
}
