using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.RateLimiting;

namespace WebAPI.Controllers
{
    /// <summary>
    /// DataDictionaryController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class DataDictionaryController : ControllerBase
    {
        private readonly DataDictionaryControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDictionaryController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        public DataDictionaryController(DataDictionaryControllerLogic controllerLogic, IUserProvider userProvider)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Gets the complete data dictionary.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="displaySource">The display source.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{omniClientId}")]
        public async Task<DataDictionaryDTO> Get(Guid omniClientId, DisplaySource displaySource = DisplaySource.PlanitClientAlias, CancellationToken cancellationToken = default)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(omniClientId, user.Id, cancellationToken, displaySource);
        }

        /// <summary>
        /// Gets the broadcast calendar columns.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{omniClientId}")]
        public async Task<ICollection<DataDictionaryColumnDetailsDTO>> GetBroadcastCalendarColumns(
            Guid omniClientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetBroadcastCalendarColumnsAsync(omniClientId, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the client calendar columns.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{omniClientId}")]
        public async Task<ICollection<DataDictionaryColumnDetailsDTO>> GetClientCalendarColumns(
            Guid omniClientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetClientCalendarColumnsAsync(omniClientId, user.Id, cancellationToken);
        }

        /// <summary>
        /// Update the client details by ClientId.
        /// </summary>
        /// <param name="omniClientId">The omni client id identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task UpdateClientDetailsByOmniClientId(Guid omniClientId, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await controllerLogic.UpdateClientDetailsByOmniClientIdAsync(omniClientId, user.Id, cancellationToken);
        }

        /// <summary>
        /// Refresh the DataDictionary Tables data manually.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        //[EnableRateLimiting("fixed")]
        public async Task<string> RefreshDataDictionaryTables(CancellationToken cancellationToken)
        {
            return await controllerLogic.RefreshDataDictionaryTablesAsync(cancellationToken);
        }
    }
}