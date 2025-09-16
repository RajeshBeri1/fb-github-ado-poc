using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IClientControllerLogic
    /// </summary>
    public interface IClientControllerLogic
    {
        /// <summary>
        /// Create the client asynchronous.
        /// </summary>
        /// <param name="omniClient">The omniclient.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<bool> CreateAsync(OmniClientInfoDTO omniClient, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Client Details by Omni Guid.
        /// </summary>
        /// <param name="id">The client omni guid.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<OmniClientInfoDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Lists the clients.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<OmniClientInfoListDTO> GetListAsync(OmniClientSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Create OmniClient associated with client
        /// </summary>
        /// <param name="omniClient">omniClient</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateOmniClientAsync(OmniClientInfoDTO omniClient, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Update the client details asynchronous.
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="omniClient">omniClient</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateClientAsync(Guid clientId, OmniClientInfoDTO omniClient, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Soft delere the client asynchronous.
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteClientAsync(Guid clientId, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Delete the Omni client asynchronous.
        /// </summary>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteOmniClientAsync(Guid omniClientId, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets by client details by name synchronous.
        /// </summary>
        /// <param name="name">The client name</param>
        /// <param name="userId">The userId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<ClientDTO> GetClientByNameAsync(string name, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Create the client mapping asynchronous.
        /// </summary>
        /// <param name="clientMappingCreateDTO">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task CreateClientMappingAsync(ClientMappingCreateDTO clientMappingCreateDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Update the client mapping asynchronous.
        /// </summary>
        /// <param name="clientMappingUpdateDTO">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task UpdateClientMappingAsync(ClientMappingUpdateDTO clientMappingUpdateDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Update Omniclient parent by omni guid asynchronous.
        /// </summary>
        /// <param name="id">The client omni guid.</param>
        /// <param name="parentId">The parentId identifier.</param>
        /// <param name="userId">The userId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<OmniClientInfoDTO> UpdateParentAsync(Guid id, Guid? parentId, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Update Include Source MediaTools Data asynchronous.
        /// </summary>
        /// <param name="clientId">The clientId</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="userId">The userId</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<ClientDTO> UpdateIncludeSourceMediaToolsDataAsync(string clientId, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Get the Client Mapping list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        Task<List<ClientMappingDTO>> GetAllClientMappingListAsync(CancellationToken cancellationToken, bool removed = false);

        /// <summary>
        /// Get all the omni client details by using omniClientId asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="cancellationToken">The cancellation token. Token</param>
        Task<OmniClientInfoDTO> GetAllOmniClientDetailsByOmniClientId(Guid omniClientId, CancellationToken cancellationToken);
    }
}
