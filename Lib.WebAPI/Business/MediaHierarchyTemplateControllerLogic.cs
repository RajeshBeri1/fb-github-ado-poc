using AutoMapper;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Extensions;
using Lib.WebAPI.Models.Flowchart;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// MediaHierarchyControllerLogic
    /// </summary>
    public class MediaHierarchyTemplateControllerLogic : IMediaHierarchyTemplateControllerLogic
    {
        private readonly ILog<IMediaHierarchyTemplateControllerLogic> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="MediaHierarchyTemplateControllerLogic" /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="log">The log.</param>
        /// <param name="portalDbContext">The portal database context.</param>
        public MediaHierarchyTemplateControllerLogic(
            IPortalUnitOfWork portal,
            IMapper mapper,
            SecurityLogic securityLogic,
            ILog<IMediaHierarchyTemplateControllerLogic> log,
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
        /// <param name="mediaHierarchyTemplate">The media hierarchy template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<MediaHierarchyTemplateDetailsDTO> CreateAsync(
            MediaHierarchyTemplateCreateDTO mediaHierarchyTemplate, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(mediaHierarchyTemplate.OmniClientId, user.Id, cancellationToken);

            var template = mapper.Map<MediaHierarchyTemplate>(mediaHierarchyTemplate);

            template.CreatedByUserId = user.Id;
            template.ModifiedByUserId = user.Id;

            log.Add(LogLevel.Information, $"Creating media hierarchy template: {template.Id} by user: {template.CreatedByUserId}");

            template = await portal.MediaHierarchyTemplates.AddAsync(template, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<MediaHierarchyTemplateDetailsDTO>(template);

            result.CreatedByUser = result.ModifiedByUser = mapper.Map<UserInfoDTO>(user);

            return result;
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<MediaHierarchyTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.MediaHierarchyTemplates.GetByIdAsync(id, cancellationToken);

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Deleting media hierarchy template: {template.Id} by user: {userId}");

            template = await portal.MediaHierarchyTemplates.SoftDeleteAsync(id, cancellationToken);

            await portal.SaveChangesAsync(cancellationToken);

            return mapper.Map<MediaHierarchyTemplateInfoDTO>(template);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<MediaHierarchyTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var result = await portal.MediaHierarchyTemplates.GetByIdAsync(id, cancellationToken);

            if (result.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(result.OmniClientId.Value, userId, cancellationToken);
            }

            var dto = mapper.Map<MediaHierarchyTemplateDetailsDTO>(result);

            // Migrate the Inflight Overlay to List
            dto?.Definition?.Levels?.ToList().ForEach(mh =>
            {
                mh?.Settings?.ToList().ForEach(s =>
                    {
                        s.MigrateInflightOverlayToList();
                    }
                    );
            });
            return dto;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The named search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<MediaHierarchyTemplateInfoListDTO> GetAsync(MediaHierarchyTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(search.OmniClientId, userId, cancellationToken);

            var (items, totalCount) = await portal.MediaHierarchyTemplates.SearchAllAsync<MediaHierarchyTemplateInfoDTO>(
                search, cancellationToken, x => (x.OmniClientId == null || x.OmniClientId == search.OmniClientId) && x.Removed == search.Removed);
            items = items.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.ModifiedDate).ToList();
            return new MediaHierarchyTemplateInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="mediaHierarchyTemplate">The media hierarchy template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<MediaHierarchyTemplateDetailsDTO> UpdateAsync(MediaHierarchyTemplateUpdateDTO mediaHierarchyTemplate, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portal.MediaHierarchyTemplates.GetByIdAsync(mediaHierarchyTemplate.Id, cancellationToken);

            if (template.OmniClientId != null)
            {
                await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            }

            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);

            log.Add(LogLevel.Information, $"Updating media hierarchy template: {template.Id} by user: {template.ModifiedByUserId}");

            // Update the template Inflight Overlay for migration -- Placeholder code
            //mediaHierarchyTemplate.Definition.Levels.ToList().ForEach(mh =>
            //{
            //    mh.Settings?.ToList().ForEach(s =>
            //    {
            //        s.MigrateInflightOverlayToList();
            //    });
            //});

            template = mapper.Map(mediaHierarchyTemplate, template);

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
        public async Task<MediaHierarchyTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var template = await portalDbContext.MediaHierarchyTemplates.FirstOrDefaultAsync(x => x.Id == id && x.Removed, cancellationToken) ?? throw new KeyNotFoundException($"Media hierarchy template with '{id}' not found.");

            await securityLogic.CheckUserHasClientAccessAsync(template.OmniClientId.Value, userId, cancellationToken);
            securityLogic.CheckIsOwner(template.CreatedByUserId, userId);
            template.Removed = false;
            log.Add(LogLevel.Information, $"Restoring media hierarchy template: {template.Id} by user: {userId}");
            await portal.SaveChangesAsync(cancellationToken);
            return mapper.Map<MediaHierarchyTemplateInfoDTO>(template);
        }
    }
}