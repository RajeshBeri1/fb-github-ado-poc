using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IFooterTemplateControllerLogic
    /// </summary>
    public interface IFooterTemplateControllerLogic
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="footerTemplate">The footer template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FooterTemplateDetailsDTO> CreateAsync(FooterTemplateCreateDTO footerTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FooterTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FooterTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FooterTemplateInfoListDTO> GetAsync(FooterTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="footerTemplate">The footer template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FooterTemplateDetailsDTO> UpdateAsync(FooterTemplateUpdateDTO footerTemplate, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Restores the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FooterTemplateInfoDTO> RestoreAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    }
}