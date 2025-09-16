using System.Net;
using Lib.Annalect.Models;
using Lib.Common.Business;
using Throw;

namespace Lib.Annalect.Business
{
    /// <summary>
    /// AnnalectApiClient
    /// </summary>
    public class AnnalectApiClient
    {
        /// <summary>
        /// The client name
        /// </summary>
        public const string ClientName = nameof(AnnalectApiClient);

        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnalectApiClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public AnnalectApiClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Gets the user profile asynchronous.
        /// </summary>
        /// <param name="anSid">An sid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<UserProfileResponse> GetUserProfileAsync(string anSid, CancellationToken cancellationToken)
        {
            using var client = httpClientFactory.CreateClient(ClientName);

            var url = $"/api/appdata/omni_profile/?ANsid={anSid}";

            using var response = await client.GetAsync(url, cancellationToken);

            response.ThrowIfNull().IfFalse(x => x.StatusCode == HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = Json.Deserialize<UserProfileResponse>(json);

            result.ThrowIfNull();

            return result;
        }

        /// <summary>
        /// Validates the session asynchronous.
        /// </summary>
        /// <param name="anSid">An sid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ValidateSessionResponse> ValidateSessionAsync(string anSid, CancellationToken cancellationToken)
        {
            using var client = httpClientFactory.CreateClient(ClientName);

            var url = $"/ssoapiexec?func=user/session/{anSid}&verb=get";

            using var response = await client.GetAsync(url, cancellationToken);

            response.ThrowIfNull().IfFalse(x => x.StatusCode == HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = Json.Deserialize<ValidateSessionResponse>(json);

            result.ThrowIfNull();

            return result;
        }

        /// <summary>
        /// Extend the session asynchronous.
        /// </summary>
        /// <param name="anSid">An sid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ValidateSessionResponse> ExtendSessionAsync(string anSid, CancellationToken cancellationToken)
        {
            using var client = httpClientFactory.CreateClient(ClientName);
            var url = $"/am/amapi/user/session/{anSid}";
            using var response = await client.GetAsync(url, cancellationToken);
            response.ThrowIfNull().IfFalse(x => x.StatusCode == HttpStatusCode.OK);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = Json.Deserialize<ValidateSessionResponse>(json);
            result.ThrowIfNull();
            return result;
        }

        /// <summary>
        /// Get Omni Client Details asynchronous.
        /// </summary>
        /// <param name="id">An omniClientId.</param>
        /// <param name="anSid">An sid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<Client> GetClientDetailsAsync(Guid id, string anSid, CancellationToken cancellationToken)
        {
            var profile= await GetUserProfileAsync(anSid, cancellationToken);
            return profile.Clients.ToList().Find(x => x.ClientId == id.ToString());
        }

        /// <summary>
        /// Get Omni Client List asynchronous.
        /// </summary>
        /// <param name="id">An omniClientId.</param>
        /// <param name="anSid">An sid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<ClientDetailsList>> GetOtherClientListAsync(Guid id, string anSid, CancellationToken cancellationToken)
        {
            var profile = await GetUserProfileAsync(anSid, cancellationToken);
            return profile.Clients
                           .Where(x => x.ClientId != id.ToString())
                           .Select(p => new ClientDetailsList()
                           {
                               ClientId = Guid.Parse(p.ClientId),
                               Name = p.OrgName,
                           }).ToList();
        }
    }
}