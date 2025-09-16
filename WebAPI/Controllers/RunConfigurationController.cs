using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// RunConfigurationController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class RunConfigurationController : ControllerBase
    {
        private readonly RunConfigurationControllerLogic controllerLogic;
        private readonly IUserProvider userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunConfigurationController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        public RunConfigurationController(RunConfigurationControllerLogic controllerLogic, IUserProvider userProvider)
        {
            this.controllerLogic = controllerLogic;
            this.userProvider = userProvider;
        }

        /// <summary>
        /// Creates the specified run configuration.
        /// </summary>
        /// <param name="runConfiguration">The run configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<RunConfigurationDetailsDTO> Create(RunConfigurationCreateDTO runConfiguration, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.CreateAsync(runConfiguration, user, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified run configuration.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<RunConfigurationInfoDTO> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.DeleteAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Gets the specified run configuration.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("[action]/{id}")]
        public async Task<RunConfigurationDetailsDTO> Get(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(id, user.Id, cancellationToken);
        }

        /// <summary>
        /// Lists the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<RunConfigurationInfoListDTO> List(RunConfigurationSearchDTO search, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.GetAsync(search.FlowchartTemplateId, search, user.Id, cancellationToken);
        }

        /// <summary>
        /// Updates the specified run configuration.
        /// </summary>
        /// <param name="runConfiguration">The run configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<RunConfigurationDetailsDTO> Update(RunConfigurationUpdateDTO runConfiguration, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await controllerLogic.UpdateAsync(runConfiguration, user.Id, cancellationToken);
        }
    }
}