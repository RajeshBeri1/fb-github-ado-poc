using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.Consts;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Okta.AspNetCore;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// UserControllerLogic
    /// </summary>
    public class UserControllerLogic
    {
        private readonly OktaConfig oktaConfig;
        private readonly IPortalUnitOfWork portal;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControllerLogic" /> class.
        /// </summary>
        /// <param name="oktaConfig">The okta authentication.</param>
        /// <param name="portal">The portal.</param>
        public UserControllerLogic(OktaConfig oktaConfig, IPortalUnitOfWork portal)
        {
            this.oktaConfig = oktaConfig;
            this.portal = portal;
        }

        /// <summary>
        /// Logins the asynchronous.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="isDevelopment">if set to <c>true</c> [is development].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<string> LoginAsync(LoginDTO login, bool isDevelopment, CancellationToken cancellationToken)
        {
            oktaConfig.Enabled.Throw("What are you doing?! Okta authentication is enabled. Give up or try harder.").IfTrue();

            /*
            isDevelopment.Throw("Activate Okta authentication in production.").IfFalse();
            */

            (login.UserName == "Wendy.Lator" && login.Password == "Trans4mation")
                .Throw(() => new AuthenticationException("Wrong user name or password.")).IfFalse();

            var user = (await portal.Users.GetAsync(cancellationToken, count: 1)).FirstOrDefault()
                .ThrowIfNull(() => new Exception($"No user in database available."));

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();

            handler.OutboundClaimTypeMap[ClaimTypes.NameIdentifier] = "sub";

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.NameIdentifier, user.Name),
                    // new Claim(ClaimTypes.Email, user.EMail),
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(WebApiConsts.JWTCommunicationKey)),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var token = handler.CreateToken(descriptor);
            return $"{OktaDefaults.ApiAuthenticationScheme} {handler.WriteToken(token)}";
        }
    }
}