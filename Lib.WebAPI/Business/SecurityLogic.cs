using System.Security.Authentication;
using Lib.Common.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.Models;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// SecurityLogic
    /// </summary>
    public class SecurityLogic
    {
        private static readonly Func<string, Func<Exception>> AuthException = (x) => () => new AuthenticationException(x);
        private readonly OmniAuthConfig omniAuthConfig;
        private readonly IPortalUnitOfWork portal;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityLogic" /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="omniAuthConfig">The omni authentication configuration.</param>
        public SecurityLogic(IPortalUnitOfWork portal, OmniAuthConfig omniAuthConfig)
        {
            this.portal = portal;
            this.omniAuthConfig = omniAuthConfig;
        }

        /// <summary>
        /// Checks the is owner.
        /// </summary>
        /// <param name="createdByUserId">The created by user identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="System.ArgumentException">
        /// Owner is a different user.
        /// </exception>
        public void CheckIsOwner(Guid createdByUserId, Guid userId)
        {
            if (createdByUserId != userId)
            {
                throw new ArgumentException($"Owner is a different user.");
            }
        }

        /// <summary>
        /// Checks the user has client access asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task CheckUserHasClientAccessAsync(Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            if (!omniAuthConfig.ClientIdCheck)
            {
                return;
            }

            var guids = await GetAllowedOmniClientIds(userId, cancellationToken);

            guids.Throw(AuthException("User has no allowed clients.")).IfEmpty();

            guids.Any(x => x == omniClientId).Throw(AuthException($"User is not allowed for {omniClientId}.")).IfFalse();
        }

        /// <summary>
        /// Gets the allowed omni client ids.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<Guid>> GetAllowedOmniClientIds(Guid userId, CancellationToken cancellationToken)
        {
            var user = await portal.Users.GetByIdAsync(userId, cancellationToken);
            var guids = Json.Deserialize<List<Guid>>(user.AllowedClientGuids) ?? new List<Guid>();

            return guids;
        }

        /// <summary>
        /// Gets the client identifier by unique identifier asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> GetClientIdByGuidAsync(Guid omniClientId, CancellationToken cancellationToken)
        {
            return (await portal.OmniClients.GetByIdAsync(omniClientId, cancellationToken)).Client.ClientId;
        }

        /// <summary>
        /// Gets the client name by unique identifier asynchronous.
        /// </summary>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> GetClientNameByGuidAsync(Guid omniClientId, CancellationToken cancellationToken)
        {
            return (await portal.OmniClients.GetByIdAsync(omniClientId, cancellationToken)).Client.Name;
        }
    }
}