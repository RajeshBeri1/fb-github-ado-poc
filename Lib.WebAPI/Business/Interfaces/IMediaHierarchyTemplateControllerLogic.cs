using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IMediaHierarchyTemplateControllerLogic
    /// </summary>
    public interface IMediaHierarchyTemplateControllerLogic
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="mediaHierarchyTemplate">The media hierarchy template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyTemplateDetailsDTO> CreateAsync(
            MediaHierarchyTemplateCreateDTO mediaHierarchyTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyTemplateInfoListDTO> GetAsync(MediaHierarchyTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="mediaHierarchyTemplate">The media hierarchy template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyTemplateDetailsDTO> UpdateAsync(MediaHierarchyTemplateUpdateDTO mediaHierarchyTemplate, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Restores the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<MediaHierarchyTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    }
}