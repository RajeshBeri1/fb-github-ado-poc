using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// ITotalsTemplateControllerLogic
    /// </summary>
    public interface ITotalsTemplateControllerLogic
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="totalsTemplate">The totals template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TotalsTemplateDetailsDTO> CreateAsync(TotalsTemplateCreateDTO totalsTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TotalsTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TotalsTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TotalsTemplateInfoListDTO> GetAsync(TotalsTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="totalsTemplate">The totals template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<TotalsTemplateDetailsDTO> UpdateAsync(TotalsTemplateUpdateDTO totalsTemplate, Guid userId, CancellationToken cancellationToken);
    }
}