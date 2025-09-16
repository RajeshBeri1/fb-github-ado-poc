using System.Collections.ObjectModel;
using AutoMapper;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models.Flowchart;
using Microsoft.Extensions.Logging;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// RunConfigurationControllerLogic
    /// </summary>
    public class RunConfigurationControllerLogic
    {
        private readonly ILog<RunConfigurationControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunConfigurationControllerLogic"
        /// /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portalDbContext.</param>
        public RunConfigurationControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<RunConfigurationControllerLogic> log,
            IPortalDbContext portalDbContext)
        {
            this.portal = portal;
            this.mapper = mapper;
            this.securityLogic = securityLogic;
            this.log = log;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="runConfiguration">The run configuration.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RunConfigurationDetailsDTO> CreateAsync(RunConfigurationCreateDTO runConfiguration, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.GetByIdAsync(runConfiguration.FlowchartTemplateId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, user.Id, cancellationToken);

            var configuration = mapper.Map<RunConfiguration>(runConfiguration);

            configuration.CreatedByUserId = configuration.ModifiedByUserId = user.Id;

            log.Add(LogLevel.Information, $"Creating run configuration: {configuration.Id} for template {template.Id} by user: {configuration.CreatedByUserId}");

            int runConfigCount = portalDbContext.RunConfigurations.Where(x => x.FlowchartTemplateId == runConfiguration.FlowchartTemplateId).Count();

            configuration.Version = runConfigCount > 0 ? runConfigCount + 1 : configuration.Version;

            configuration = await portal.RunConfigurations.AddAsync(configuration, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<RunConfigurationDetailsDTO>(configuration);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Creates a default run configuration to ensure one is available to enter the run experience as soon as a template is created
        /// </summary>
        /// <param name="templateId"> the templateId.</param>
        /// <param name="user">the user.</param>
        /// <param name="cancellationToken">the cancellationToken.</param>
        public async Task<RunConfigurationDetailsDTO> CreateDefaultRunConfigurationAsync(Guid templateId, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.GetByIdAsync(templateId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, user.Id, cancellationToken);

            var runConfiguration = new RunConfigurationCreateDTO
            {
                FlowchartTemplateId = templateId,
                MediaHierarchyLevels = new Collection<MediaHierarchyLevelBase>(),
                Name = $"{template.Name}-Default",
                RunRestrictions = new Collection<RunRestriction>(),
            };

            return await CreateAsync(runConfiguration, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RunConfigurationInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var configuration = await portal.RunConfigurations.GetByIdAsync(id, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(configuration.FlowchartTemplate.OmniClientId, userId, cancellationToken);

            securityLogic.CheckIsOwner(configuration.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting run configuration: {configuration.Id} by user: {userId}");

            configuration.FlowchartTemplate.RunConfigurations.Remove(configuration);
            portal.RunConfigurations.Remove(x => x.Id == id);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<RunConfigurationInfoDTO>(configuration);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RunConfigurationDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.RunConfigurations.GetByIdAsync(id, cancellationToken);
            await securityLogic.CheckUserHasClientAccessAsync(result.FlowchartTemplate.OmniClientId, userId, cancellationToken);
            return mapper.Map<RunConfigurationDetailsDTO>(result);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="flowchartTemplateId">The flowchart template identifier.</param>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RunConfigurationInfoListDTO> GetAsync(Guid flowchartTemplateId, RunConfigurationSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FlowchartTemplates.GetByIdAsync(flowchartTemplateId, cancellationToken);

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.RunConfigurations.SearchAsync<RunConfigurationInfoDTO>(
                search, cancellationToken, x => x.FlowchartTemplateId == flowchartTemplateId);

            return new RunConfigurationInfoListDTO
            {
                Items = items.OrderByDescending(x=> x.CreatedDate),
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="runConfiguration">The run configuration.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RunConfigurationDetailsDTO> UpdateAsync(RunConfigurationUpdateDTO runConfiguration, Guid userId, CancellationToken cancellationToken)
        {
            var configuration = await portal.RunConfigurations.GetByIdAsync(runConfiguration.Id, cancellationToken);

            log.Add(LogLevel.Information, $"Updating run configuration: {configuration.Id} by user: {configuration.ModifiedByUserId}");

            await securityLogic.CheckUserHasClientAccessAsync(configuration.FlowchartTemplate.OmniClientId, userId, cancellationToken);

            securityLogic.CheckIsOwner(configuration.CreatedByUserId, userId);

            configuration = mapper.Map(runConfiguration, configuration);

            configuration.ModifiedDate = DateTime.UtcNow;

            int runConfigCount = portalDbContext.RunConfigurations.Where(x => x.FlowchartTemplateId == configuration.FlowchartTemplateId).Count();

            configuration.Version = runConfigCount > 0 ? runConfigCount + 1 : configuration.Version;

            // configuration.Version++;

            await portal.SaveChangesAsync(cancellationToken);

            return await GetAsync(configuration.Id, userId, cancellationToken);
        }
    }
}