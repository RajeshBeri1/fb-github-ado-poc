using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Lib.Annalect.Business;
using Lib.Annalect.Models;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;
using Throw;

namespace WebAPI.Controllers
{
    /// <summary>
    /// ClientController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class ClientController : ControllerBase
    {
        private readonly IClientControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;
        public const string AuthenticationScheme = "ANsid";
        private readonly AnnalectApiClient apiClient;
        private readonly DefaultTemplateUpdater defaultTemplateUpdater;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientController" /> class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="apiClient">The API client.</param>
        /// <param name="defaultTemplateUpdater">The default template updater.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>"
        public ClientController(
            IUserProvider userProvider,
            IClientControllerLogic controllerLogic,
            AnnalectApiClient apiClient,
            DefaultTemplateUpdater defaultTemplateUpdater,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.userProvider = userProvider;
            this.controllerLogic = controllerLogic;
            this.apiClient = apiClient;
            this.defaultTemplateUpdater = defaultTemplateUpdater;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Lists the clients.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [ProducesResponseType(typeof(OmniClientInfoListDTO), 200)]
        [HttpPost("[action]")]
        public async Task<OmniClientInfoListDTO> List(OmniClientSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetListAsync(search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Creates a new client with omni id mapping.
        /// </summary>
        /// <param name="omniClient">The omniClient info.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(OmniClientInfoDTO omniClient, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            bool data = await controllerLogic.CreateAsync(omniClient, user.Id, cancellationToken);
            if (data)
            {
                bool isUpdate = false;
                // trigger background job to update default template for all clients in the system
                string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
                var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
                await defaultTemplateUpdater.RunAsync(ansidValue, backgroundJobCancellation.Token, omniClient.Id, isUpdate);
                return Ok("Client created successfully");
            }
            else
            {
                return Ok("Client creation operation failed");
            }
        }

        /// <summary>
        /// Get Client Details by Omni Guid.
        /// </summary>
        /// <param name="id">The client omni guid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [ProducesResponseType(typeof(OmniClientInfoDTO), 200)]
        [HttpPost("[action]")]
        public async Task<OmniClientInfoDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var header = Request.Headers[HeaderNames.Authorization].ToString();
            var split = header.Split(" ", 2);
            var details = await apiClient.GetClientDetailsAsync(id, split[1], cancellationToken);
            var client = await controllerLogic.GetAsync(id, user.Id, cancellationToken);
            var data = new OmniClientInfoDTO() { Client = new ClientInfoDTO() { Name = details.OrgName, ClientId = client.Client.ClientId, IncludeSourceMediaToolsData = client.Client.IncludeSourceMediaToolsData } };
            data.Version = client.Version;
            data.Country = client.Country;
            data.Id = client.Id;
            return data;
        }

        /// <summary>
        /// Create the Omni client through client name asynchronous.
        /// </summary>
        /// <param name="omniClient">omniClient</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateOmniClient(OmniClientInfoDTO omniClient, CancellationToken cancellationToken)
        {
            bool isUpdate = false;
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            string ansidValue = GetClaimValue(OmniAuthenticationHandler.AuthenticationScheme);
            var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
            await controllerLogic.CreateOmniClientAsync(omniClient, user.Id, cancellationToken);
            await defaultTemplateUpdater.RunAsync(ansidValue, backgroundJobCancellation.Token, omniClient.Id, isUpdate);
            return Ok("Omni Client created successfully");
        }

        /// <summary>
        /// Update the client details asynchronous.
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="omniClient">omniClient</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Update(Guid clientId, OmniClientInfoDTO omniClient, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await controllerLogic.UpdateClientAsync(clientId, omniClient, user.Id, cancellationToken);
            return Ok("Client upated successfully");
        }

        /// <summary>
        /// Soft Delete the client asynchronous.
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid clientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await controllerLogic.DeleteClientAsync(clientId, user.Id, cancellationToken);
            return Ok("Client deleted successfully");
        }

        /// <summary>
        /// Delete the omni client
        /// </summary>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteOmniClient(Guid omniClientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await controllerLogic.DeleteOmniClientAsync(omniClientId, user.Id, cancellationToken);
            return Ok("Omni client deleted successfully");
        }

        /// <summary>
        /// Get client details by name
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [ProducesResponseType(typeof(ClientDTO), 200)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetClientByName(string name, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return Ok(await controllerLogic.GetClientByNameAsync(name, user.Id, cancellationToken));
        }

        /// <summary>
        /// Update the parent client.
        /// </summary>
        /// <param name="id">omni client id</param>
        /// <param name="parentId">parent omni client id</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [ProducesResponseType(typeof(OmniClientInfoDTO), 200)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateParent(Guid id, Guid parentId = default, CancellationToken cancellationToken = default)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            var result = await controllerLogic.UpdateParentAsync(id, parentId, user.Id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a client mapping.
        /// </summary>
        /// <param name="clientMappingCreateDTO">The omniClient info.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateClientMapping(ClientMappingCreateDTO clientMappingCreateDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await controllerLogic.CreateClientMappingAsync(clientMappingCreateDTO, cancellationToken);
            return Ok("Client mapping created successfully");
        }

        /// <summary>
        /// Update a client mapping.
        /// </summary>
        /// <param name="clientMappingUpdateDTO">The omniClient info.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateClientMapping(ClientMappingUpdateDTO clientMappingUpdateDTO, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await controllerLogic.UpdateClientMappingAsync(clientMappingUpdateDTO, cancellationToken);
            return Ok("Client mapping updated successfully");
        }

        /// <summary>
        /// Update Include Source MediaTools Data.
        /// </summary>
        /// <param name="clientId">The clientId</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateIncludeSourceMediaToolsData(string clientId, bool includeSourceMediaToolsData, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return Ok(await controllerLogic.UpdateIncludeSourceMediaToolsDataAsync(clientId, includeSourceMediaToolsData, user.Id, cancellationToken));
        }

        /// <summary>
        /// Get all the Client Mapping list.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        [HttpPost("[action]")]
        public async Task<List<ClientMappingDTO>> GetAllClientMappingList(CancellationToken cancellationToken, bool removed = false)
        {
            return await controllerLogic.GetAllClientMappingListAsync(cancellationToken, removed);
        }

        private string? GetClaimValue(string key)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(key)?.Value;
        }
    }
}