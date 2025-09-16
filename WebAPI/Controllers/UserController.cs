using System.Diagnostics.CodeAnalysis;
using Lib.Annalect.Business;
using Lib.Annalect.Models;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// UserController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class UserController : ControllerBase
    {
        private readonly UserControllerLogic controllerLogic;
        private readonly IWebHostEnvironment environment;
        private readonly IUserProvider userProvider;
        private readonly AnnalectApiClient apiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="apiClient">The apiClient.</param>
        public UserController(IUserProvider userProvider, UserControllerLogic controllerLogic, IWebHostEnvironment environment, AnnalectApiClient apiClient)
        {
            this.userProvider = userProvider;
            this.controllerLogic = controllerLogic;
            this.environment = environment;
            this.apiClient = apiClient;
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]")]
        public async Task<UserDetailsDTO> Current(CancellationToken cancellationToken)
        {
            return await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
        }

        /// <summary>
        /// Logins the specified user. For development only.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<string> Login(LoginDTO login, CancellationToken cancellationToken)
        {
            return await controllerLogic.LoginAsync(login, environment.IsDevelopment(), cancellationToken);
        }

        /// <summary>
        /// Validate the current user token
        /// </summary>
        /// <param name="token">Omni session token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{token}")]
        [AllowAnonymous]
        public async Task<ValidateSessionResponse> ValidateToken(string token, CancellationToken cancellationToken)
        {
            return await apiClient.ValidateSessionAsync(token, cancellationToken);
        }

        /// <summary>
        /// Extend the current user token expiration time
        /// </summary>
        /// <param name="token">Omni session token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]/{token}")]
        public async Task<ValidateSessionResponse> ExtendSession(string token, CancellationToken cancellationToken)
        {
            return await apiClient.ExtendSessionAsync(token, cancellationToken);
        }
    }
}