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
    /// CalendarOverlayTemplateControllerLogic
    /// </summary>
    public class CalendarOverlayTemplateControllerLogic : ICalendarOverlayTemplateControllerLogic
    {
        private readonly ILog<CalendarOverlayTemplateControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="CalendarOverlayTemplateControllerLogic" /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portal database context.</param>
        public CalendarOverlayTemplateControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<CalendarOverlayTemplateControllerLogic> log,
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
        /// <param name="overlayTemplate">The overlay template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarOverlayTemplateDetailsDTO> CreateAsync(CalendarOverlayTemplateCreateDTO overlayTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(overlayTemplate.OmniClientId, user.Id, cancellationToken);

            var template = mapper.Map<CalendarOverlayTemplate>(overlayTemplate);

            template.CreatedByUserId = user.Id;
            template.ModifiedByUserId = user.Id;

            log.Add(LogLevel.Information, $"Creating calendar overlay template: {template.Id} by user: {template.CreatedByUserId}");

            template = await portal.CalendarOverlayTemplates.AddAsync(template, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<CalendarOverlayTemplateDetailsDTO>(template);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarOverlayTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.CalendarOverlayTemplates.GetByIdAsync(id, cancellationToken);

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting calendar overlay template: {template.Id} by user: {userId}");

            template = await portal.CalendarOverlayTemplates.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<CalendarOverlayTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarOverlayTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.CalendarOverlayTemplates.GetByIdAsync(id, cancellationToken);

            if (result.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId.Value, userId, cancellationToken);
            }

            return mapper.Map<CalendarOverlayTemplateDetailsDTO>(result);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarOverlayTemplateInfoListDTO> GetAsync(CalendarOverlayTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(search.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.CalendarOverlayTemplates.SearchAllAsync<CalendarOverlayTemplateInfoDTO>(
                search, cancellationToken, x => (x.OmniClientId == null || x.OmniClientId == search.OmniClientId) && x.Removed == search.Removed);
            items = items.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.ModifiedDate).ToList();
            return new CalendarOverlayTemplateInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="overlayTemplate">The overlay template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarOverlayTemplateDetailsDTO> UpdateAsync(CalendarOverlayTemplateUpdateDTO overlayTemplate, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.CalendarOverlayTemplates.GetByIdAsync(overlayTemplate.Id, cancellationToken);

            log.Add(LogLevel.Information, $"Updating calendar overlay template: {template.Id} by user: {template.ModifiedByUserId}");

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            template = mapper.Map(overlayTemplate, template);

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
        public async Task<CalendarOverlayTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {

            var template = await portalDbContext.CalendarOverlayTemplates.FirstOrDefaultAsync(x => x.Id == id && x.Removed, cancellationToken) ?? throw new KeyNotFoundException($"Calendar overlay template with '{id}' not found.");

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
            template.Removed = false;
            log.Add(LogLevel.Information, $"Restoring calendar overlay template: {template.Id} by user: {userId}");
            await portal.SaveChangesAsync(cancellationToken);
            return mapper.Map<CalendarOverlayTemplateInfoDTO>(template);
        }
    }
}