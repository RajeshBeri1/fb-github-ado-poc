using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Lib.Annalect.Models;
using Lib.Athena.Consts;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using System.Collections.Generic;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// ClientControllerLogic
    /// </summary>
    public class ClientControllerLogic : IClientControllerLogic
    {
        private readonly OmniAuthConfig omniAuthConfig;
        private readonly IPortalUnitOfWork portal;
        private readonly SecurityLogic securityLogic;
        private readonly ILog<IClientControllerLogic> log;
        private readonly IPortalDbContext portalDbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientControllerLogic" /> class.
        /// </summary>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="omniAuthConfig">The omni authentication configuration.</param>
        /// <param name="portal">The portal.</param>
        /// <param name="log">The log.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="portalDbContext">The portal database context.</param>"
        public ClientControllerLogic(
            SecurityLogic securityLogic,
            OmniAuthConfig omniAuthConfig,
            IPortalUnitOfWork portal,
            ILog<IClientControllerLogic> log,
            IMapper mapper,
            IPortalDbContext portalDbContext)
        {
            this.securityLogic = securityLogic;
            this.omniAuthConfig = omniAuthConfig;
            this.portal = portal;
            this.log = log;
            this.mapper = mapper;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<OmniClientInfoListDTO> GetListAsync(OmniClientSearchDTO search, Guid userId, CancellationToken cancellationToken)
        {
            var guids = await securityLogic.GetAllowedOmniClientIds(userId, cancellationToken);

            var (items, totalCount) = await portal.OmniClients.GetAsync<OmniClientInfoDTO>(
                cancellationToken, search.Start, search.Count,
                x => (search.SearchText == null || x.Client.Name.Contains(search.SearchText)) &&
                (!omniAuthConfig.ClientIdCheck || (guids.Any() && guids.Contains(x.Id))),
                orderByName: search.OrderBy, ascending: search.OrderAscending);
            foreach (var item in items)
            {
                var clientId = portalDbContext.OmniClients.FirstOrDefault(x => x.Id == item.Id).ClientId;
                var clientMappingDetails = portalDbContext.ClientMapping.FirstOrDefault(x => x.ClientId == clientId);
                if (clientMappingDetails != null)
                {
                    item.Version = clientMappingDetails.Version;
                }
            }
            return new OmniClientInfoListDTO
            {
                Items = items,
                TotalCount = totalCount,
            };
        }

        /// <summary>
        /// Create the client asynchronous.
        /// </summary>
        /// <param name="omniClient">The omniclient.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CreateAsync(OmniClientInfoDTO omniClient, Guid userId, CancellationToken cancellationToken)
        {
            bool result = false;
            var omniClients = new List<OmniClient>()
            {
                new()
                {
                    Country=omniClient.Country, Id=omniClient.Id,
                },
            };
            var clientDetails = await portal.Clients.AddAsync(
                new()
                {
                    ClientId = omniClient.Client.ClientId,
                    Name = omniClient.Client.Name,
                    OmniClients = omniClients,
                    IncludeSourceMediaToolsData = omniClient.Client.IncludeSourceMediaToolsData,
                }, cancellationToken);

            var clientMappingDetails = await portal.ClientMapping.AddAsync(
                new ClientMapping
                {
                    ClientId = clientDetails.Id,
                    Version = omniClient.Version ?? 1,
                    Removed = false,
                }, cancellationToken);
            result = (clientDetails != null && clientMappingDetails != null) ? true : false;

            await portal.SaveChangesAsync(cancellationToken);
            return result;
        }


        /// <summary>
        /// Create the Omni Client asynchronous.
        /// </summary>
        /// <param name="omniClient">The omniclient.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task CreateOmniClientAsync(OmniClientInfoDTO omniClient, Guid userId, CancellationToken cancellationToken)
        {
            var client = await portal.Clients.GetByNameAsync(omniClient.Client.Name, cancellationToken);
            client.ThrowIfNull(nameof(client));
            log.Add(LogLevel.Information, $"Creating omni client: {omniClient.Id} by user: {userId}");
            await portal.OmniClients.AddAsync(
               new()
               {
                   Client = client,
                   Country = omniClient.Country,
                   Id = omniClient.Id,
               }, cancellationToken);
            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update the client details asynchronous.
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="omniClient">omniClient</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateClientAsync(Guid clientId, OmniClientInfoDTO omniClient, Guid userId, CancellationToken cancellationToken)
        {
            var client = await portal.Clients.GetByIdAsync(clientId, cancellationToken);
            client.ThrowIfNull(nameof(client));
            client.Name = omniClient.Client.Name;
            client.ClientId = omniClient.Client.ClientId;
            client.IncludeSourceMediaToolsData = omniClient.Client.IncludeSourceMediaToolsData;
            log.Add(LogLevel.Information, $"Updating client: {clientId} by user: {userId}");

            var clientIdExists = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == client.Id);
            if (clientIdExists == null)
            {
                await portal.ClientMapping.AddAsync(
                new ClientMapping
                {
                    ClientId = client.Id,
                    Version = omniClient.Version ?? 1,
                    Removed = false,
                }, cancellationToken);
            }
            else
            {
                clientIdExists.Version = omniClient.Version ?? 1;
                clientIdExists.Removed = omniClient.Removed;
            }

            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Soft Delete the client asynchronous.
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteClientAsync(Guid clientId, Guid userId, CancellationToken cancellationToken)
        {
            var client = await portal.Clients.GetByIdAsync(clientId, cancellationToken);
            client.ThrowIfNull(nameof(client));
            log.Add(LogLevel.Information, $"Deleting client: {clientId} by user: {userId}");
            await portal.Clients.SoftDeleteAsync(clientId, cancellationToken);
            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Delete Omni Client asynchronous.
        /// </summary>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="userId">userId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DeleteOmniClientAsync(Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            var client = await portal.OmniClients.GetByIdAsync(omniClientId, cancellationToken);
            client.ThrowIfNull(nameof(client));
            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);
            log.Add(LogLevel.Information, $"Deleting omni client: {omniClientId} by user: {userId}");
            portal.OmniClients.Remove(oc => oc.Id == omniClientId);
            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Gets by omni guid asynchronous.
        /// </summary>
        /// <param name="id">The client omni guid.</param>
        /// <param name="userId">The userId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<OmniClientInfoDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            await securityLogic.CheckUserHasClientAccessAsync(id, userId, cancellationToken);

            var omniClientsDetails = await portal.OmniClients.GetByIdAsync<OmniClientInfoDTO>(id, cancellationToken);
            var clientId = portalDbContext.OmniClients.FirstOrDefault(x => x.Id == omniClientsDetails.Id)?.ClientId;
            var clientMappingDetails = portalDbContext.ClientMapping.FirstOrDefault(x => x.ClientId == clientId);
            if (clientMappingDetails != null)
            {
                omniClientsDetails.Version = clientMappingDetails.Version;
            }

            return omniClientsDetails ?? new OmniClientInfoDTO();
        }

        /// <summary>
        /// Gets by client details by name synchronous.
        /// </summary>
        /// <param name="name">The client name</param>
        /// <param name="userId">The userId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ClientDTO> GetClientByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
        {
            name.ThrowIfNull(nameof(name));
            return await portal.Clients.GetByNameAsync<ClientDTO>(name, cancellationToken);
        }

        /// <summary>
        /// Update Omniclient parent by omni guid asynchronous.
        /// </summary>
        /// <param name="id">The client omni guid.</param>
        /// <param name="parentId">The parentId identifier.</param>
        /// <param name="userId">The userId identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<OmniClientInfoDTO> UpdateParentAsync(Guid id, Guid? parentId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Updating omni client: {id} by user: {userId}.The parent client is {parentId}");

            var client = await portal.OmniClients.GetByIdAsync(id, cancellationToken);
            client.ThrowIfNull(nameof(client));

            if (parentId.HasValue && parentId.Value != Guid.Empty)
            {
                if (client.Id == parentId)
                {
                    throw new InvalidOperationException("Client cannot be its own parent");
                }
                var parentClient = await portal.OmniClients.GetByIdAsync(parentId.Value, cancellationToken);
                parentClient.ThrowIfNull(nameof(parentClient));
                client.ParentId = parentId;
            }
            else
            {
                client.ParentId = null;
            }
            await portal.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<OmniClientInfoDTO>(client);
            return result;
        }

        /// <summary>
        /// Create the client mapping details asynchronous.
        /// </summary>
        /// <param name="clientMappingCreateDTO">ClientMappingUpdateDTO</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateClientMappingAsync(ClientMappingCreateDTO clientMappingCreateDTO, CancellationToken cancellationToken)
        {
            var clientIdExists = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == clientMappingCreateDTO.ClientId);
            if (clientIdExists == null)
            {
                await portal.ClientMapping.AddAsync(
                new ClientMapping
                {
                    ClientId = clientMappingCreateDTO.ClientId,
                    Version = clientMappingCreateDTO.Version ?? 1,
                    Removed = false,
                }, cancellationToken);
            }
            log.Add(LogLevel.Information, $"Creatting client mapping: {clientMappingCreateDTO.ClientId}");
            await portalDbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update the client mapping details asynchronous.
        /// </summary>
        /// <param name="clientMappingUpdateDTO">ClientMappingUpdateDTO</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateClientMappingAsync(ClientMappingUpdateDTO clientMappingUpdateDTO, CancellationToken cancellationToken)
        {
            var clientMapping = await portalDbContext.ClientMapping.FirstOrDefaultAsync(x => x.ClientId == clientMappingUpdateDTO.ClientId, cancellationToken);
            clientMapping.ThrowIfNull(nameof(clientMapping));

            clientMapping.Version = clientMappingUpdateDTO.Version ?? 1;
            clientMapping.Removed = clientMappingUpdateDTO.Removed;
            clientMapping.UseParentFlightDates = clientMappingUpdateDTO.UseParentFlightDates;
            log.Add(LogLevel.Information, $"Updating client mapping: {clientMappingUpdateDTO.ClientId}");
            await portal.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Update IncludeSourceMediaToolsData field for client table  synchronous.
        /// </summary>
        /// <param name="clientId">The clientId</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="userId">The userId</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<ClientDTO> UpdateIncludeSourceMediaToolsDataAsync(string clientId, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            clientId.ThrowIfNull(nameof(clientId));
            includeSourceMediaToolsData.ThrowIfNull(nameof(includeSourceMediaToolsData));
            var data = await portalDbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
            data.ThrowIfNull(nameof(data));
            data.IncludeSourceMediaToolsData = includeSourceMediaToolsData;
            await portalDbContext.SaveChangesAsync(cancellationToken);
            return mapper.Map<ClientDTO>(data);
        }

        /// <summary>
        /// Get the Client Mapping list asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        public async Task<List<ClientMappingDTO>> GetAllClientMappingListAsync(CancellationToken cancellationToken, bool removed = false)
        {
            List<ClientMappingCreateDTO> clientIds = await portalDbContext.OmniClients.Where(x => !x.Removed).Select(t => new ClientMappingCreateDTO { ClientId = t.ClientId, Removed = t.Removed }).AsNoTracking().Distinct().ToListAsync(cancellationToken);
            var clientMappingsList = await portalDbContext.ClientMapping.Where(x => !x.Removed).ToListAsync();

            var dataInsertIntoClientMapping = clientMappingsList.Count > 0 ? clientIds.Where(x => !clientMappingsList.Any(y => y.ClientId == x.ClientId)).ToList() : clientIds;

            if (dataInsertIntoClientMapping.Count > 0)
            {
                dataInsertIntoClientMapping.ForEach(x => x.Version = 1);
                var insertClientMapping = mapper.Map<List<ClientMapping>>(dataInsertIntoClientMapping);
                await portal.ClientMapping.AddRangeAsync(insertClientMapping, cancellationToken);
                await portal.SaveChangesAsync(cancellationToken);
            }

            var defaultTemplateDetails = await portalDbContext.ClientMapping.Where(x => !x.Removed).Select(t => new ClientMappingDTO { Id = t.Id, ClientId = t.ClientId, Version = t.Version, Removed = t.Removed }).AsNoTracking().Distinct().ToListAsync(cancellationToken);
            defaultTemplateDetails.ThrowIfNull(nameof(defaultTemplateDetails));
            return defaultTemplateDetails;
        }

        /// <summary>
        /// Get all the omni client details by using omniClientId asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="cancellationToken">The cancellation token. Token</param>
        public async Task<OmniClientInfoDTO> GetAllOmniClientDetailsByOmniClientId(Guid omniClientId, CancellationToken cancellationToken)
        {
            var query = await (from c in portalDbContext.Clients
                               join oc in portalDbContext.OmniClients on c.Id equals oc.ClientId
                               join cm in portalDbContext.ClientMapping on c.Id equals cm.ClientId
                               where oc.Id == omniClientId
                               select new OmniClientInfoDTO
                               {
                                   Client = new ClientInfoDTO
                                   {
                                       Name = c.Name,
                                       ClientId = oc.ClientId.ToString(),
                                       IncludeSourceMediaToolsData = c.IncludeSourceMediaToolsData,
                                   },
                                   Version = cm.Version <= 0 ? 1 : cm.Version,
                                   Id = oc.Id,
                                   Removed = cm.Removed,
                                   UseParentFlightDates = cm.UseParentFlightDates,
                               }).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            return query ?? new OmniClientInfoDTO();
        }
    }
}