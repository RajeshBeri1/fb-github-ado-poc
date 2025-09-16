using AutoMapper;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// FooterTemplateControllerLogic
    /// </summary>
    public class FooterTemplateControllerLogic : IFooterTemplateControllerLogic
    {
        private readonly ILog<IFooterTemplateControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FooterTemplateControllerLogic" />
        /// class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portal database context.</param>
        public FooterTemplateControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<IFooterTemplateControllerLogic> log,
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
        /// <param name="footerTemplate">The footer template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FooterTemplateDetailsDTO> CreateAsync(FooterTemplateCreateDTO footerTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(footerTemplate.OmniClientId, user.Id, cancellationToken);

            var template = mapper.Map<FooterTemplate>(footerTemplate);

            template.CreatedByUserId = user.Id;
            template.ModifiedByUserId = user.Id;

            log.Add(LogLevel.Information, $"Creating footer template: {template.Id} by user: {template.CreatedByUserId}");

            template = await portal.FooterTemplates.AddAsync(template, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<FooterTemplateDetailsDTO>(template);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FooterTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FooterTemplates.GetByIdAsync(id, cancellationToken);

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting footer template: {template.Id} by user: {userId}");

            template = await portal.FooterTemplates.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<FooterTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FooterTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.FooterTemplates.GetByIdAsync(id, cancellationToken);

            if (result.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId.Value, userId, cancellationToken);
            }

            return mapper.Map<FooterTemplateDetailsDTO>(result);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FooterTemplateInfoListDTO> GetAsync(FooterTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(search.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.FooterTemplates.SearchAllAsync<FooterTemplateInfoDTO>(
                      search, cancellationToken, x => (x.OmniClientId == null || x.OmniClientId == search.OmniClientId) && x.Removed == search.Removed);
            items = items.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.ModifiedDate).ToList();
            return new FooterTemplateInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="footerTemplate">The footer template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FooterTemplateDetailsDTO> UpdateAsync(FooterTemplateUpdateDTO footerTemplate, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.FooterTemplates.GetByIdAsync(footerTemplate.Id, cancellationToken);

            log.Add(LogLevel.Information, $"Updating footer template: {template.Id} by user: {template.ModifiedByUserId}");

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            template = mapper.Map(footerTemplate, template);

            template.ModifiedDate = DateTime.UtcNow;
            template.Version++;

            await portal.SaveChangesAsync(cancellationToken);

            return await GetAsync(template.Id, userId, cancellationToken);
        }

        /// <summary>
        /// Restores the asynchronous.
        /// </summary>
        /// <param name="id">The identifier of the template to restore.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FooterTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portalDbContext.FooterTemplates.FirstOrDefaultAsync(x => x.Id == id && x.Removed, cancellationToken) ?? throw new KeyNotFoundException($"Footer template with '{id}' not found.");

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
            template.Removed = false;
            log.Add(LogLevel.Information, $"Restoring footer template: {template.Id} by user: {userId}");
            await portal.SaveChangesAsync(cancellationToken);
            return mapper.Map<FooterTemplateInfoDTO>(template);
        }
    }
}