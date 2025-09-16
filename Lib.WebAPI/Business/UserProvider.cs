using System.Security.Authentication;
using System.Security.Claims;
using AutoMapper;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// UserProvider
    /// </summary>
    public class UserProvider : IUserProvider
    {
        private static readonly Func<string, Func<Exception>> AuthException = (x) => () => new AuthenticationException(x);
        private readonly IMapper mapper;
        private readonly OktaConfig oktaConfig;
        private readonly OmniAuthConfig omniAuthConfig;
        private readonly IPortalUnitOfWork portal;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProvider" /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="omniAuthConfig">The omni authentication configuration.</param>
        /// <param name="oktaConfig">The okta configuration.</param>
        public UserProvider(IPortalUnitOfWork portal, IMapper mapper, OmniAuthConfig omniAuthConfig, OktaConfig oktaConfig)
        {
            this.portal = portal;
            this.mapper = mapper;
            this.oktaConfig = oktaConfig;
            this.omniAuthConfig = omniAuthConfig;
        }

        /// <summary>
        /// Gets the current asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<UserDetailsDTO> GetCurrentAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            var claimsIdentity = user?.Identity as ClaimsIdentity;
            claimsIdentity.ThrowIfNull(AuthException($"Identity is not a claims identity"));

            if (omniAuthConfig.Enabled && !oktaConfig.Enabled)
            {
                var personId = claimsIdentity.Claims?.FirstOrDefault(x => x.Type == OmniAuthenticationHandler.ClaimPersonId)?.Value;
                Guid.TryParse(personId, out var userId).Throw().IfFalse();

                return await portal.Users.GetByIdAsync<UserDetailsDTO>(userId, cancellationToken);
            }

            var userName = claimsIdentity.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            userName.ThrowIfNull(AuthException($"Claim {nameof(ClaimTypes.NameIdentifier)} is null or empty.")).IfEmpty();

            return await AddOrGetAsync(userName, cancellationToken);
        }

        private async Task<UserDetailsDTO> AddOrGetAsync(string userName, CancellationToken cancellationToken)
        {
            try
            {
                return await portal.Users.GetByNameAsync<UserDetailsDTO>(userName, cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                var user = await portal.Users.AddOrUpdateAsync(
                    new User
                    {
                        DisplayName = userName,
                        Name = userName,
                    }, x => x.Name, cancellationToken);

                return mapper.Map<UserDetailsDTO>(user);
            }
        }
    }
}