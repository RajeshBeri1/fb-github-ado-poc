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
    /// CalendarTemplateControllerLogic
    /// </summary>
    public class CalendarTemplateControllerLogic : ICalendarTemplateControllerLogic
    {
        private readonly ILog<ICalendarTemplateControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarTemplateControllerLogic"
        /// /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portal database context.</param>
        public CalendarTemplateControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<ICalendarTemplateControllerLogic> log,
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
        /// <param name="calendarTemplate">The calendar template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarTemplateDetailsDTO> CreateAsync(CalendarTemplateCreateDTO calendarTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(calendarTemplate.OmniClientId, user.Id, cancellationToken);

            var template = mapper.Map<CalendarTemplate>(calendarTemplate);

            template.CreatedByUserId = user.Id;
            template.ModifiedByUserId = user.Id;

            log.Add(LogLevel.Information, $"Creating calendar template: {template.Id} by user: {template.CreatedByUserId}");

            template = await portal.CalendarTemplates.AddAsync(template, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<CalendarTemplateDetailsDTO>(template);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.CalendarTemplates.GetByIdAsync(id, cancellationToken);

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting calendar template: {template.Id} by user: {userId}");

            template = await portal.CalendarTemplates.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<CalendarTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.CalendarTemplates.GetByIdAsync(id, cancellationToken);

            if (result.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId.Value, userId, cancellationToken);
            }

            return mapper.Map<CalendarTemplateDetailsDTO>(result);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The named search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarTemplateInfoListDTO> GetAsync(CalendarTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(search.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.CalendarTemplates.SearchAllAsync<CalendarTemplateInfoDTO>(
                search, cancellationToken, x => (x.OmniClientId == null || x.OmniClientId == search.OmniClientId) && x.Removed == search.Removed);
            items = items.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.ModifiedDate).ToList();
            return new CalendarTemplateInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="calendarTemplate">The calendar template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<CalendarTemplateDetailsDTO> UpdateAsync(
            CalendarTemplateUpdateDTO calendarTemplate, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.CalendarTemplates.GetByIdAsync(calendarTemplate.Id, cancellationToken);

            log.Add(LogLevel.Information, $"Updating calendar template: {template.Id} by user: {template.ModifiedByUserId}");

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            template = mapper.Map(calendarTemplate, template);

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
        public async Task<CalendarTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portalDbContext.CalendarTemplates.FirstOrDefaultAsync(x => x.Id == id && x.Removed, cancellationToken) ?? throw new KeyNotFoundException($"Calendar template with '{id}' not found.");

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
            template.Removed = false;
            log.Add(LogLevel.Information, $"Restoring calendar template: {template.Id} by user: {userId}");
            await portal.SaveChangesAsync(cancellationToken);
            return mapper.Map<CalendarTemplateInfoDTO>(template);
        }
    }
}