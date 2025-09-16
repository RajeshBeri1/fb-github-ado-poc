using AutoMapper;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.Extensions.Logging;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// SummaryTemplateControllerLogic
    /// </summary>
    public class SummaryTemplateControllerLogic
    {
        private readonly ILog<SummaryTemplateControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryTemplateControllerLogic"
        /// /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        public SummaryTemplateControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<SummaryTemplateControllerLogic> log)
        {
            this.portal = portal;
            this.mapper = mapper;
            this.securityLogic = securityLogic;
            this.log = log;
        }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="summaryTemplate">The summary template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<SummaryTemplateDetailsDTO> CreateAsync(SummaryTemplateCreateDTO summaryTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(summaryTemplate.OmniClientId, user.Id, cancellationToken);

            var template = mapper.Map<SummaryTemplate>(summaryTemplate);
            template.CreatedByUserId = template.ModifiedByUserId = user.Id;

            log.Add(LogLevel.Information, $"Creating summary template: {template.Id} by user: {template.CreatedByUserId}");

            template = await portal.SummaryTemplates.AddAsync(template, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<SummaryTemplateDetailsDTO>(template);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<SummaryTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.SummaryTemplates.GetByIdAsync(id, cancellationToken);

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting summary template: {template.Id} by user: {userId}");

            template = await portal.SummaryTemplates.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<SummaryTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<SummaryTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.SummaryTemplates.GetByIdAsync(id, cancellationToken);

            if (result.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId.Value, userId, cancellationToken);
            }

            return mapper.Map<SummaryTemplateDetailsDTO>(result);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<SummaryTemplateInfoListDTO> GetAsync(SummaryTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(search.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.SummaryTemplates.SearchAsync<SummaryTemplateInfoDTO>(
                search, cancellationToken, x => x.OmniClientId == null || x.OmniClientId == search.OmniClientId);

            return new SummaryTemplateInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="summaryTemplate">The summary template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<SummaryTemplateDetailsDTO> UpdateAsync(SummaryTemplateUpdateDTO summaryTemplate, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.SummaryTemplates.GetByIdAsync(summaryTemplate.Id, cancellationToken);

            log.Add(LogLevel.Information, $"Updating summary template: {template.Id} by user: {template.ModifiedByUserId}");

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            template = mapper.Map(summaryTemplate, template);

            template.ModifiedDate = DateTime.UtcNow;
            template.Version++;

            await portal.SaveChangesAsync(cancellationToken);

            return await GetAsync(template.Id, userId, cancellationToken);
        }
    }
}