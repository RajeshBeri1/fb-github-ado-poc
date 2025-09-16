using System.Security.Claims;
using System.Text.Encodings.Web;
using AutoMapper;
using Lib.Annalect.Business;
using Lib.Annalect.Models;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// OmniAuthenticationHandler
    /// </summary>
    public class OmniAuthenticationHandler : AuthenticationHandler<OmniAuthenticationSchemeOptions>
    {
        /// <summary>
        /// The authentication scheme
        /// </summary>
        public const string AuthenticationScheme = "ANsid";

        /// <summary>
        /// The claim person identifier
        /// </summary>
        public const string ClaimPersonId = "OmniPersonId";

        private readonly AnnalectApiClient apiClient;
        private readonly OmniAuthConfig authConfig;
        private readonly ICacheLogic cache;
        private readonly ILog<OmniAuthenticationHandler> log;
        private readonly IMapper mapper;
        private readonly IPortalUnitOfWork portal;

        /// <summary>
        /// Initializes a new instance of the <see cref="OmniAuthenticationHandler" />
        /// class.
        /// </summary>
        /// <param name="options">The monitor for the options instance.</param>
        /// <param name="logger">
        /// The <see cref="T:Microsoft.Extensions.Logging.ILoggerFactory" />.
        /// </param>
        /// <param name="encoder">
        /// The <see cref="T:System.Text.Encodings.Web.UrlEncoder" />.
        /// </param>
        /// <param name="clock">
        /// The <see cref="T:Microsoft.AspNetCore.Authentication.ISystemClock" />.
        /// </param>
        /// <param name="apiClient">The API client.</param>
        /// <param name="authConfig">The authentication configuration.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="portal">The portal.</param>
        /// <param name="log">The log.</param>
        public OmniAuthenticationHandler(
            IOptionsMonitor<OmniAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            AnnalectApiClient apiClient,
            OmniAuthConfig authConfig,
            ICacheLogic cache,
            IMapper mapper,
            IPortalUnitOfWork portal,
            ILog<OmniAuthenticationHandler> log)
            : base(options, logger, encoder, clock)
        {
            this.apiClient = apiClient;
            this.authConfig = authConfig;
            this.cache = cache;
            this.mapper = mapper;
            this.portal = portal;
            this.log = log;
        }

        /// <summary>
        /// Allows derived types to handle authentication.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Microsoft.AspNetCore.Authentication.AuthenticateResult" />.
        /// </returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                Request.Headers.ContainsKey(HeaderNames.Authorization).Throw(x => new Exception($"Missing {HeaderNames.Authorization} header.")).IfFalse();

                var header = Request.Headers[HeaderNames.Authorization].ToString();

                var split = header.Split(" ", 2);

                (split[0] != AuthenticationScheme).Throw(x => new Exception($"Invalid authentication scheme, expected: {AuthenticationScheme}")).IfTrue();
                split[1].ThrowIfNull(x => new Exception($"Missing authentication parameter.")).IfEmpty();

                using var source = new CancellationTokenSource();

                var user = default(UserProfileResponse);

                if (authConfig.LiveCheck)
                {
                    // check each time against Annalect API
                    user = await apiClient.GetUserProfileAsync(split[1], source.Token);
                }
                else
                {
                    // check once and cache result, reuse cached data
                    user = await cache.GetAsync(
                        $"{AuthenticationScheme}_{split[1]}",
                        x => apiClient.GetUserProfileAsync(split[1], x), source.Token);
                }

                user.Projects.ThrowIfNull(x => new Exception($"User has no projects.")).IfEmpty();
                user.Projects.Any(x => x.ProjectId == authConfig.FlowchartProjectId)
                    .Throw(x => new Exception("User is not authorized for Flowchart project.")).IfFalse();

                var clientIds = new List<Guid>();

                if (authConfig.ClientIdCheck)
                {
                    user.Clients.ThrowIfNull(x => new Exception($"User has no clients.")).IfEmpty();

                    foreach (var client in user.Clients)
                    {
                        Guid.TryParse(client.ClientId, out var guid).Throw(x => new Exception($"Client Id {client.ClientId} is not a valid GUID.")).IfFalse();
                        clientIds.Add(guid);
                    }
                }

                Guid.TryParse(user.PersonId, out var userId).Throw(x => new Exception($"Person Id {user.PersonId} is not a valid GUID.")).IfFalse();

                var dbUser = await portal.Users.AddOrUpdateAsync(
                    new User
                    {
                        Id = userId,
                        DisplayName = user.FullName,
                        Name = user.EMail,
                        AllowedClientGuids = Json.Serialize(clientIds),
                    }, x => x.Name, source.Token);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, dbUser.Name),
                    new Claim(ClaimTypes.Name, dbUser.DisplayName),
                    new Claim(ClaimPersonId, dbUser.Id.ToString()),
                    new Claim(AuthenticationScheme,split[1]),
                };

                var claimsIdentity = new ClaimsIdentity(claims, nameof(OmniAuthenticationHandler));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Critical, ex.Message);
                return AuthenticateResult.Fail(ex.Message);
            }
        }
    }
}