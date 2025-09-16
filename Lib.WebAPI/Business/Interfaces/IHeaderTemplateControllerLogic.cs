using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IHeaderTemplateControllerLogic
    /// </summary>
    public interface IHeaderTemplateControllerLogic
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="headerTemplate">The header template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<HeaderTemplateDetailsDTO> CreateAsync(HeaderTemplateCreateDTO headerTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<HeaderTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<HeaderTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<HeaderTemplateInfoListDTO> GetAsync(HeaderTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="headerTemplate">The header template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<HeaderTemplateDetailsDTO> UpdateAsync(HeaderTemplateUpdateDTO headerTemplate, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Restores the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<HeaderTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    }
}